using System;
using System.Buffers;
using Godot;
using NovoDwarf.Godot.Slang.Utils;

namespace NovoDwarf.Godot.Slang;

public sealed class SlangComputeTask : IDisposable
{
    private readonly RenderingDevice _rd;
    private readonly Rid _shaderId;
    private readonly Rid _pipelineId;
    private readonly SlangBufferSet _buffers;
    private readonly int _localSize;

    public SlangComputeTask(string shaderResPath, int kernelIndex = 0, int localSize = 8)
    {
        _localSize = localSize;

        var shaderFile = GodotUtils.GetShader(shaderResPath);
        ThrowUtils.ThrowIfSlangError(shaderFile);
        var spirv = GodotUtils.GetSpirV(shaderFile, kernelIndex);

        (_rd, _shaderId, _pipelineId) = GodotUtils.RunOnRenderThread(() =>
        {
            var rd = RenderingServer.Singleton.CreateLocalRenderingDevice();
            var shaderId = rd.ShaderCreateFromSpirV(spirv);
            return (rd, shaderId, rd.ComputePipelineCreate(shaderId));
        });

        _buffers = new SlangBufferSet(_rd, _shaderId);
    }
    
    private bool _disposed;
    
    public float[] Dispatch(int width, int height, ReadOnlySpan<float> kernelParams, params float[][] inputs)
    {
        var groupsX = (uint)((width + _localSize - 1) / _localSize);
        var groupsY = (uint)((height + _localSize - 1) / _localSize);

        var paramsCount = kernelParams.Length;
        var paramsCopy = ArrayPool<float>.Shared.Rent(paramsCount);
        kernelParams.CopyTo(paramsCopy);

        try
        {
            return GodotUtils.RunOnRenderThread(() =>
            {
                _buffers.Ensure(width, height, paramsCopy, paramsCount, inputs);

                var list = _rd.ComputeListBegin();
               
                _rd.ComputeListBindComputePipeline(list, _pipelineId);
                _rd.ComputeListBindUniformSet(list, _buffers.UniformSetId, 0);
                _rd.ComputeListDispatch(list, groupsX, groupsY, 1);
                _rd.ComputeListEnd();
                _rd.Submit();
                _rd.Sync();

                var rawBytes = _rd.BufferGetData(_buffers.OutputBufId);
                var result = new float[width * height];
                Buffer.BlockCopy(rawBytes, 0, result, 0, rawBytes.Length);
               
                return result;
            });
        }
        finally
        {
            ArrayPool<float>.Shared.Return(paramsCopy);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        GodotUtils.RunOnRenderThread(() =>
        {
            _buffers.Dispose();
            _rd.FreeRid(_pipelineId);
            _rd.FreeRid(_shaderId);
            _rd.Free();
        });
    }
}
