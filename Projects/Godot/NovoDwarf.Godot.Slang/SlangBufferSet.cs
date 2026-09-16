using System;
using Godot;
using Godot.Collections;
using NovoDwarf.Godot.Slang.Utils;

namespace NovoDwarf.Godot.Slang;

internal sealed class SlangBufferSet : IDisposable
{
    private readonly RenderingDevice _rd;
    private readonly Rid _shaderId;
    
    internal SlangBufferSet(RenderingDevice rd, Rid shaderId)
    {
        _rd = rd;
        _shaderId = shaderId;
    }
    
    internal Rid UniformSetId => _uniformSetId;
    internal Rid OutputBufId => _outputBufId;

    private Rid _paramsBufId;
    private Rid[] _inputBufIds = [];
    private Rid _outputBufId;
    private Rid _uniformSetId;

    private int _width;
    private int _height;
    private int _paramsLength;
    private int[] _inputLengths = [];
    
    internal void Ensure(int width, int height, float[] kernelParams, int paramsCount, float[][] inputs)
    {
        if (!IsMatch(width, height, paramsCount, inputs))
        {
            Rebuild(width, height, kernelParams, paramsCount, inputs);
            return;
        }

        Upload(_paramsBufId, kernelParams, paramsCount);
        for (var i = 0; i < inputs.Length; i++)
            Upload(_inputBufIds[i], inputs[i], inputs[i].Length);
    }

    public void Dispose() => Free();

    private bool IsMatch(int width, int height, int paramsCount, float[][] inputs)
    {
        if (!_paramsBufId.IsValid || !_outputBufId.IsValid || !_uniformSetId.IsValid) return false;
        if (_width != width || _height != height || _paramsLength != paramsCount) return false;
        if (_inputBufIds.Length != inputs.Length || _inputLengths.Length != inputs.Length) return false;

        for (var i = 0; i < inputs.Length; i++)
            if (!_inputBufIds[i].IsValid || _inputLengths[i] != inputs[i].Length)
                return false;

        return true;
    }

    private void Rebuild(int width, int height, float[] kernelParams, int paramsCount, float[][] inputs)
    {
        Free();

        var paramsBufId = new Rid();
        var inputBufIds = new Rid[inputs.Length];
        var outputBufId = new Rid();
        var uniformSetId = new Rid();

        try
        {
            paramsBufId = Alloc(kernelParams, paramsCount);
            
            for (var i = 0; i < inputs.Length; i++)
                inputBufIds[i] = Alloc(inputs[i], inputs[i].Length);

            var outputByteCount = width * height * sizeof(float);
            outputBufId = _rd.StorageBufferCreate((uint)outputByteCount, new byte[outputByteCount]);

            var uniforms = new Array<RDUniform> { UniformUtils.Make(paramsBufId, 0) };
            
            for (var i = 0; i < inputBufIds.Length; i++)
                uniforms.Add(UniformUtils.Make(inputBufIds[i], i + 1));
            
            uniforms.Add(UniformUtils.Make(outputBufId, inputBufIds.Length + 1));

            uniformSetId = _rd.UniformSetCreate(uniforms, _shaderId, 0);

            _paramsBufId = paramsBufId;
            _inputBufIds = inputBufIds;
            _outputBufId = outputBufId;
            _uniformSetId = uniformSetId;
            _width = width;
            _height = height;
            _paramsLength = paramsCount;

            _inputLengths = new int[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
                _inputLengths[i] = inputs[i].Length;
        }
        catch
        {
            FreeIfValid(uniformSetId);
            FreeIfValid(outputBufId);
           
            for (var i = inputBufIds.Length - 1; i >= 0; i--)
                FreeIfValid(inputBufIds[i]);
           
            FreeIfValid(paramsBufId);
            
            throw;
        }
    }

    private Rid Alloc(float[] data, int count)
    {
        var byteCount = count * sizeof(float);
        var bytes = new byte[byteCount];
        Buffer.BlockCopy(data, 0, bytes, 0, byteCount);
        return _rd.StorageBufferCreate((uint)byteCount, bytes);
    }

    private void Upload(Rid bufferId, float[] data, int count)
    {
        var byteCount = count * sizeof(float);
        var bytes = new byte[byteCount];
        Buffer.BlockCopy(data, 0, bytes, 0, byteCount);

        var error = _rd.BufferUpdate(bufferId, 0, (uint)byteCount, bytes);
        if (error != Error.Ok)
            throw new InvalidOperationException($"Buffer upload failed: {error}.");
    }

    private void Free()
    {
        FreeIfValid(_uniformSetId);
        FreeIfValid(_outputBufId);
       
        for (var i = _inputBufIds.Length - 1; i >= 0; i--)
            FreeIfValid(_inputBufIds[i]);
       
        FreeIfValid(_paramsBufId);

        _uniformSetId = new Rid();
        _outputBufId = new Rid();
        _inputBufIds = [];
        _paramsBufId = new Rid();
        _width = 0;
        _height = 0;
        _paramsLength = 0;
        _inputLengths = [];
    }

    private void FreeIfValid(Rid rid)
    {
        if (rid.IsValid)
            _rd.FreeRid(rid);
    }
}
