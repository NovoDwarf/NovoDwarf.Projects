using Brutal;
using Brutal.VulkanApi;

namespace NovoDwarf.Brutal.Rml;

internal sealed unsafe class RmlVulkanPipeline : IDisposable
{
    private readonly Device _device;
    
    public RmlVulkanPipeline(
        Device device, 
        VkRenderPass renderPass, 
        VkDescriptorSetLayout descriptors,
        ReadOnlyMemory<byte> vertexShader,
        ReadOnlyMemory<byte> fragmentShader)
    {
        _device = device;

        var vertex = default(VkShaderModule);
        var fragment = default(VkShaderModule);

        try
        {
            vertex = CreateModule(vertexShader);
            fragment = CreateModule(fragmentShader);

            var range = new VkPushConstantRange
            {
                StageFlags = VkShaderStageFlags.VertexBit,
                Size = (ByteSize32)80
            };

            Layout = device.CreatePipelineLayout(
                new VkPipelineLayoutCreateInfo
                {
                    SetLayoutCount = 1,
                    SetLayouts = &descriptors,
                    PushConstantRangeCount = 1,
                    PushConstantRanges = &range
                },
                null);

            fixed (byte* entryPoint = "main"u8)
            {
                var stages = stackalloc VkPipelineShaderStageCreateInfo[2]
                {
                    new() { Stage = VkShaderStageFlags.VertexBit, Module = vertex, Name = entryPoint },
                    new() { Stage = VkShaderStageFlags.FragmentBit, Module = fragment, Name = entryPoint }
                };

                var binding = new VkVertexInputBindingDescription
                    { Binding = 0, Stride = (ByteSize32)20, InputRate = VkVertexInputRate.Vertex };

                var attributes = stackalloc VkVertexInputAttributeDescription[3]
                {
                    new() { Location = 0, Binding = 0, Format = VkFormat.R32G32SFloat, Offset = (ByteSize32)0 },
                    new() { Location = 1, Binding = 0, Format = VkFormat.R32G32SFloat, Offset = (ByteSize32)12 },
                    new() { Location = 2, Binding = 0, Format = VkFormat.R8G8B8A8UNorm, Offset = (ByteSize32)8 }
                };

                var vertexInput = new VkPipelineVertexInputStateCreateInfo
                {
                    VertexBindingDescriptionCount = 1,
                    VertexBindingDescriptions = &binding,
                    VertexAttributeDescriptionCount = 3,
                    VertexAttributeDescriptions = attributes
                };

                var assembly = new VkPipelineInputAssemblyStateCreateInfo
                    { Topology = VkPrimitiveTopology.TriangleList };

                var viewport = new VkPipelineViewportStateCreateInfo { ViewportCount = 1, ScissorCount = 1 };

                var rasterization = new VkPipelineRasterizationStateCreateInfo
                {
                    PolygonMode = VkPolygonMode.Fill,
                    CullMode = VkCullModeFlags.None,
                    FrontFace = VkFrontFace.CounterClockwise,
                    LineWidth = 1
                };

                var multisample = new VkPipelineMultisampleStateCreateInfo { RasterizationSamples = VkSampleCountFlags._1Bit };

                var depth = new VkPipelineDepthStencilStateCreateInfo
                {
                    DepthTestEnable = false,
                    DepthWriteEnable = false,
                    DepthCompareOp = VkCompareOp.Less
                };

                var colorAttachment = new VkPipelineColorBlendAttachmentState
                {
                    BlendEnable = true,
                    SrcColorBlendFactor = VkBlendFactor.One,
                    DstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
                    ColorBlendOp = VkBlendOp.Add,
                    SrcAlphaBlendFactor = VkBlendFactor.One,
                    DstAlphaBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
                    AlphaBlendOp = VkBlendOp.Add,
                    ColorWriteMask = VkColorComponentFlags.RBit | VkColorComponentFlags.GBit | VkColorComponentFlags.BBit | VkColorComponentFlags.ABit
                };

                var blend = new VkPipelineColorBlendStateCreateInfo { AttachmentCount = 1, Attachments = &colorAttachment };
                var dynamicStates = stackalloc VkDynamicState[2] { VkDynamicState.Viewport, VkDynamicState.Scissor };
                var dynamic = new VkPipelineDynamicStateCreateInfo { DynamicStateCount = 2, DynamicStates = dynamicStates };

                var createInfo = new VkGraphicsPipelineCreateInfo
                {
                    StageCount = 2,
                    Stages = stages,
                    VertexInputState = &vertexInput,
                    InputAssemblyState = &assembly,
                    ViewportState = &viewport,
                    RasterizationState = &rasterization,
                    MultisampleState = &multisample,
                    DepthStencilState = &depth,
                    ColorBlendState = &blend,
                    DynamicState = &dynamic,
                    Layout = Layout,
                    RenderPass = renderPass,
                    BasePipelineIndex = -1
                };

                Span<VkPipeline> pipelines = stackalloc VkPipeline[1];

                var result = device.CreateGraphicsPipelines(default, [createInfo], null, pipelines);

                Handle = pipelines[0];

                RmlVulkanDevice.Check(result, "Create graphics pipeline");
            }
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            if (!vertex.IsNull())
                device.DestroyShaderModule(vertex, null);

            if (!fragment.IsNull())
                device.DestroyShaderModule(fragment, null);
        }
    }

    public VkPipeline Handle { get; }
    public VkPipelineLayout Layout { get; }

    private bool _disposed;
    
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (!Handle.IsNull())
            _device.DestroyPipeline(Handle, null);

        if (!Layout.IsNull())
            _device.DestroyPipelineLayout(Layout, null);
    }

    private VkShaderModule CreateModule(ReadOnlyMemory<byte> code)
    {
        if (code.Length < 20 || code.Length % sizeof(int) != 0)
            throw new ArgumentException("Expected SPIR-V bytecode.", nameof(code));
       
        fixed (byte* bytes = code.Span)
        {
            return _device.CreateShaderModule(new VkShaderModuleCreateInfo
            {
                CodeSize = (ByteSize64)code.Length,
                Code = (int*)bytes
            }, null);
        }
    }
}

