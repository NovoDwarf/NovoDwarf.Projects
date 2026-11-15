using System.Numerics;
using System.Runtime.InteropServices;
using Brutal;
using Brutal.Numerics;
using Brutal.VulkanApi;
using Brutal.VulkanApi.Abstractions;
using NovoDwarf.Rml;

namespace NovoDwarf.Brutal.Rml;

public sealed unsafe class RmlVulkanRenderer : IDisposable
{
    private sealed class Buffer
    {
        public VkBuffer Handle;
        public VkDeviceMemory Memory;
        public int Capacity;
    }

    private sealed class Texture
    {
        public VkImage Image;
        public VkDeviceMemory Memory;
        public VkImageView View;
        public VkDescriptorSet Descriptor;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PushConstants
    {
        public Matrix4x4 Transform;
        public float2 Translation;
        public float2 Viewport;
    }

    private readonly RmlVulkanDevice _context;
    private readonly RmlRenderData _data;
    private readonly ReadOnlyMemory<byte> _vertexShader;
    private readonly ReadOnlyMemory<byte> _fragmentShader;
    private readonly Buffer[] _vertices;
    private readonly Buffer[] _indices;
    private readonly Dictionary<nuint, Texture> _textures = [];
    private readonly VkDescriptorSetLayout _descriptors;
    private readonly VkDescriptorPool _pool;
    private readonly VkSampler _sampler;
    private readonly VkCommandPool _commands;
    
    public RmlVulkanRenderer(RmlVulkanDevice context, RmlRenderData data, ReadOnlyMemory<byte> vertexShader, ReadOnlyMemory<byte> fragmentShader, int frames)
    {
        _context = context;
        _data = data;
        _vertexShader = vertexShader;
        _fragmentShader = fragmentShader;
        _vertices = [.. Enumerable.Range(0, frames).Select(_ => new Buffer())];
        _indices = [.. Enumerable.Range(0, frames).Select(_ => new Buffer())];
        
        var device = context.Device;
      
        try
        {
            var binding = new VkDescriptorSetLayoutBinding
            {
                Binding = 0,
                DescriptorType = VkDescriptorType.CombinedImageSampler,
                DescriptorCount = 1,
                StageFlags = VkShaderStageFlags.FragmentBit
            };
            
            _descriptors = device.CreateDescriptorSetLayout(new VkDescriptorSetLayoutCreateInfo { BindingCount = 1, Bindings = &binding }, null);
           
            var size = new VkDescriptorPoolSize { Type = VkDescriptorType.CombinedImageSampler, DescriptorCount = 512 };
            
            _pool = device.CreateDescriptorPool(
                new VkDescriptorPoolCreateInfo
                {
                    Flags = VkDescriptorPoolCreateFlags.FreeDescriptorSetBit,
                    MaxSets = 512,
                    PoolSizeCount = 1,
                    PoolSizes = &size
                },
                null);
            
            _sampler = device.CreateSampler(
                new VkSamplerCreateInfo
                {
                    MagFilter = VkFilter.Linear,
                    MinFilter = VkFilter.Linear,
                    MipmapMode = VkSamplerMipmapMode.Nearest,
                    AddressModeU = VkSamplerAddressMode.ClampToEdge,
                    AddressModeV = VkSamplerAddressMode.ClampToEdge,
                    AddressModeW = VkSamplerAddressMode.ClampToEdge,
                    MaxLod = 0
                },
                null);
           
            _commands = device.CreateCommandPool(
                new VkCommandPoolCreateInfo
                {
                    QueueFamilyIndex = context.QueueFamily,
                    Flags = VkCommandPoolCreateFlags.TransientBit
                },
                null);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    private RmlVulkanPipeline? _pipeline;
    private bool _disposed;
    
    public void ReleasePipeline()
    {
        _pipeline?.Dispose();
        _pipeline = null;
    }

    public void Recreate(VkRenderPass renderPass)
    {
        ReleasePipeline();
        _pipeline = new RmlVulkanPipeline(_context.Device, renderPass, _descriptors, _vertexShader, _fragmentShader);
    }

    public void Prepare(int frameIndex)
    {
        _data.ThrowIfFailed();
        
        foreach (var pair in _data.Textures)
        {
            if (!_textures.ContainsKey(pair.Key))
                _textures.Add(pair.Key, UploadTexture(pair.Value));
        }

        var removed = _textures.Keys.Where(key => key != 0 && !_data.Textures.ContainsKey(key)).ToArray();
      
        if (removed.Length > 0)
        {
            _context.Device.WaitIdle();
          
            foreach (var key in removed)
            {
                FreeTexture(_textures[key]);
                _textures.Remove(key);
            }
        }

        if (!_textures.ContainsKey(0))
            _textures.Add(0, UploadTexture(new RmlRenderData.Texture([255, 255, 255, 255], 1, 1)));

        var vertexCount = _data.VertexCount;
        var indexCount = _data.IndexCount;
       
        if (vertexCount == 0 || indexCount == 0)
            return;

        EnsureBuffer(_vertices[frameIndex], checked(vertexCount * sizeof(RmlVertex)), VkBufferUsageFlags.VertexBufferBit);
        EnsureBuffer(_indices[frameIndex], checked(indexCount * sizeof(int)), VkBufferUsageFlags.IndexBufferBit);
        
        var vertexMemory = Map(_vertices[frameIndex]);
        
        try
        {
            var indexMemory = Map(_indices[frameIndex]);
           
            try
            {
                var vertices = new Span<RmlVertex>(vertexMemory, vertexCount);
                var indices = new Span<int>(indexMemory, indexCount);
                var vertexOffset = 0;
                var indexOffset = 0;
               
                foreach (var draw in _data.Draws)
                {
                    draw.Geometry.Vertices.CopyTo(vertices[vertexOffset..]);
                    draw.Geometry.Indices.CopyTo(indices[indexOffset..]);
                  
                    vertexOffset += draw.Geometry.Vertices.Length;
                    indexOffset += draw.Geometry.Indices.Length;
                }
            }
            finally
            {
                _context.Device.UnmapMemory(_indices[frameIndex].Memory);
            }
        }
        finally
        {
            _context.Device.UnmapMemory(_vertices[frameIndex].Memory);
        }
    }

    public void Draw(CommandBuffer commands, int frameIndex, VkExtent2D extent)
    {
        if (_data.Draws.Count == 0)
            return;
        
        var pipeline = _pipeline!;
       
        commands.BindPipeline(VkPipelineBindPoint.Graphics, pipeline.Handle);
        commands.SetViewport(0, [new VkViewport { Width = extent.Width, Height = extent.Height, MinDepth = 0, MaxDepth = 1 }]);
        commands.BindVertexBuffers(0, [_vertices[frameIndex].Handle], [default]);
        commands.BindIndexBuffer(_indices[frameIndex].Handle, default, VkIndexType.UInt32);
        
        var firstIndex = 0;
        var firstVertex = 0;
       
        foreach (var draw in _data.Draws)
        {
            var left = Math.Clamp(draw.XClip, 0, extent.Width);
            var top = Math.Clamp(draw.YClip, 0, extent.Height);
          
            var right = draw.WidthClip < 0
                ? extent.Width
                : Math.Clamp((long)draw.XClip + draw.WidthClip, left, extent.Width);
            
            var bottom = draw.HeightClip < 0
                ? extent.Height
                : Math.Clamp((long)draw.YClip + draw.HeightClip, top, extent.Height);
           
            if (right > left && bottom > top && draw.Geometry.Indices.Length > 0)
            {
                commands.SetScissor(
                    0,
                    [
                        new VkRect2D
                        {
                            Offset = new VkOffset2D { X = left, Y = top },
                            Extent = new VkExtent2D { Width = (int)(right - left), Height = (int)(bottom - top) }
                        }
                    ]);
               
                commands.BindDescriptorSets(VkPipelineBindPoint.Graphics, pipeline.Layout, 0, [_textures[draw.Texture].Descriptor], []);
                
                Span<PushConstants> push =
                [
                    new()
                    {
                        Transform = draw.Transform,
                        Translation = new float2(draw.X, draw.Y),
                        Viewport = new float2(extent.Width, extent.Height)
                    }
                ];
                
                commands.PushConstants(pipeline.Layout, VkShaderStageFlags.VertexBit, 0, MemoryMarshal.AsBytes(push));
                commands.DrawIndexed(draw.Geometry.Indices.Length, 1, firstIndex, firstVertex, 0);
            }

            firstIndex += draw.Geometry.Indices.Length;
            firstVertex += draw.Geometry.Vertices.Length;
        }
    }

    private void EnsureBuffer(Buffer buffer, int bytes, VkBufferUsageFlags usage)
    {
        if (buffer.Capacity >= bytes)
            return;
       
        FreeBuffer(buffer);
        
        var device = _context.Device;
       
        buffer.Capacity = Math.Max(bytes, 65536);
        buffer.Handle = device.CreateBuffer(new VkBufferCreateInfo 
            {
                Size = (ByteSize64)buffer.Capacity,
                Usage = usage,
                SharingMode = VkSharingMode.Exclusive
            },
            null);
       
        var requirements = device.GetBufferMemoryRequirements(buffer.Handle);
      
        buffer.Memory = device.AllocateMemory(new VkMemoryAllocateInfo
            {
                AllocationSize = requirements.Size,
                MemoryTypeIndex = _context.PhysicalDevice.FindMemoryType(requirements.MemoryTypeBits, VkMemoryPropertyFlags.HostVisibleBit | VkMemoryPropertyFlags.HostCoherentBit)
            },
            null);
        device.BindBufferMemory(buffer.Handle, buffer.Memory, default);
    }

    private void* Map(Buffer buffer)
    {
        void* mapped;
        
        RmlVulkanDevice.Check(_context.Device.PInvoke.vkMapMemory.Invoke(
                _context.Device.Handle,
                buffer.Memory,
                default,
                (ByteSize64)buffer.Capacity,
                0,
                &mapped),
            "Map RmlUi buffer");
        
        return mapped;
    }

    private Texture UploadTexture(RmlRenderData.Texture source)
    {
        var device = _context.Device;
        var texture = new Texture();
        var staging = new Buffer();
        var commands = default(CommandBuffer);
        var allocated = false;
      
        try
        {
            EnsureBuffer(staging, source.Pixels.Length, VkBufferUsageFlags.TransferSrcBit);
            var mapped = Map(staging);
            
            source.Pixels.CopyTo(new Span<byte>(mapped, source.Pixels.Length));
            device.UnmapMemory(staging.Memory);
            texture.Image = device.CreateImage(new VkImageCreateInfo
                {
                    ImageType = VkImageType._2D,
                    Format = VkFormat.R8G8B8A8UNorm,
                    Extent = new VkExtent3D { Width = source.Width, Height = source.Height, Depth = 1 },
                    MipLevels = 1,
                    ArrayLayers = 1,
                    Samples = VkSampleCountFlags._1Bit,
                    Tiling = VkImageTiling.Optimal,
                    Usage = VkImageUsageFlags.TransferDstBit | VkImageUsageFlags.SampledBit,
                    SharingMode = VkSharingMode.Exclusive,
                    InitialLayout = VkImageLayout.Undefined
                },
                null);
            
            var requirements = device.GetImageMemoryRequirements(texture.Image);
            
            texture.Memory = device.AllocateMemory(new VkMemoryAllocateInfo
                {
                    AllocationSize = requirements.Size,
                    MemoryTypeIndex = _context.PhysicalDevice.FindMemoryType(requirements.MemoryTypeBits, VkMemoryPropertyFlags.DeviceLocalBit)
                },
                null);
           
            device.BindImageMemory(texture.Image, texture.Memory, default);
            commands = device.AllocateCommandBuffer(new VkCommandBufferAllocateInfo { CommandPool = _commands, Level = VkCommandBufferLevel.Primary });
            allocated = true;
            
            commands.Begin(new VkCommandBufferBeginInfo { Flags = VkCommandBufferUsageFlags.OneTimeSubmitBit });
           
            var range = new VkImageSubresourceRange
            { 
                AspectMask = VkImageAspectFlags.ColorBit, 
                LevelCount = 1, 
                LayerCount = 1 
            };
            
            var barrier = new VkImageMemoryBarrier
            {
                Image = texture.Image,
                OldLayout = VkImageLayout.Undefined,
                NewLayout = VkImageLayout.TransferDstOptimal,
                DstAccessMask = VkAccessFlags.TransferWriteBit,
                SrcQueueFamilyIndex = -1,
                DstQueueFamilyIndex = -1,
                SubresourceRange = range
            };
            
            commands.PipelineBarrier(
                VkPipelineStageFlags.TopOfPipeBit,
                VkPipelineStageFlags.TransferBit,
                0,
                [],
                [],
                [barrier]);
            
            commands.CopyBufferToImage(
                staging.Handle,
                texture.Image,
                VkImageLayout.TransferDstOptimal,
                [new VkBufferImageCopy
                    {
                        ImageSubresource = new VkImageSubresourceLayers { AspectMask = VkImageAspectFlags.ColorBit, LayerCount = 1 },
                        ImageExtent = new VkExtent3D { Width = source.Width, Height = source.Height, Depth = 1 }
                    }
                ]);
         
            barrier.OldLayout = VkImageLayout.TransferDstOptimal;
            barrier.NewLayout = VkImageLayout.ShaderReadOnlyOptimal;
            barrier.SrcAccessMask = VkAccessFlags.TransferWriteBit;
            barrier.DstAccessMask = VkAccessFlags.ShaderReadBit;
            
            commands.PipelineBarrier(
                VkPipelineStageFlags.TransferBit,
                VkPipelineStageFlags.FragmentShaderBit,
                0,
                [],
                [],
                [barrier]);
           
            commands.End();
           
            var command = commands.Handle;
           
            _context.Queue.Submit([new VkSubmitInfo { CommandBufferCount = 1, CommandBuffers = &command }], default);
            _context.Queue.WaitIdle();
           
            texture.View = device.CreateImageView(new VkImageViewCreateInfo 
                {
                    Image = texture.Image,
                    ViewType = VkImageViewType._2D,
                    Format = VkFormat.R8G8B8A8UNorm,
                    SubresourceRange = range
                },
                null);
            
            var layout = _descriptors;
            
            Span<VkDescriptorSet> descriptor = stackalloc VkDescriptorSet[1];
            
            device.AllocateDescriptorSets(new VkDescriptorSetAllocateInfo { DescriptorPool = _pool, DescriptorSetCount = 1, SetLayouts = &layout }, descriptor);
            
            texture.Descriptor = descriptor[0];
           
            var image = new VkDescriptorImageInfo
            {
                Sampler = _sampler, 
                ImageView = texture.View, 
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal
            };
            
            device.UpdateDescriptorSets([new VkWriteDescriptorSet
                    {
                        DstSet = texture.Descriptor,
                        DstBinding = 0,
                        DescriptorCount = 1,
                        DescriptorType = VkDescriptorType.CombinedImageSampler,
                        ImageInfo = &image
                    }],
                []);
            
            return texture;
        }
        catch
        {
            device.WaitIdle();
            FreeTexture(texture);
            throw;
        }
        finally
        {
            if (allocated)
                device.FreeCommandBuffers(_commands, [commands]);
            FreeBuffer(staging);
        }
    }

    private void FreeBuffer(Buffer buffer)
    {
        if (!buffer.Handle.IsNull())
            _context.Device.DestroyBuffer(buffer.Handle, null);
       
        if (!buffer.Memory.IsNull())
            _context.Device.FreeMemory(buffer.Memory, null);
       
        buffer.Handle = default;
        buffer.Memory = default;
        buffer.Capacity = 0;
    }

    private void FreeTexture(Texture texture)
    {
        var device = _context.Device;
       
        if (!texture.Descriptor.IsNull())
        {
            var descriptor = texture.Descriptor;
            RmlVulkanDevice.Check(device.PInvoke.vkFreeDescriptorSets.Invoke(device.Handle, _pool, 1, &descriptor), "Free RmlUi descriptor");
        }
        
        if (!texture.View.IsNull())
            device.DestroyImageView(texture.View, null);
      
        if (!texture.Image.IsNull())
            device.DestroyImage(texture.Image, null);
      
        if (!texture.Memory.IsNull())
            device.FreeMemory(texture.Memory, null);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        
        _disposed = true;
       
        var device = _context.Device;
       
        device.WaitIdle();
       
        ReleasePipeline();
       
        foreach (var texture in _textures.Values)
            FreeTexture(texture);
       
        foreach (var buffer in _vertices.Concat(_indices))
            FreeBuffer(buffer);
       
        if (!_commands.IsNull())
            device.DestroyCommandPool(_commands, null);
      
        if (!_sampler.IsNull())
            device.DestroySampler(_sampler, null);
       
        if (!_pool.IsNull())
            device.DestroyDescriptorPool(_pool, null);
       
        if (!_descriptors.IsNull())
            device.DestroyDescriptorSetLayout(_descriptors, null);
    }
}