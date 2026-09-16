using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NovoDwarf.Primitives.Models;

public sealed class Field : IDisposable
{
    private readonly float[] _values;
    private readonly int _length;
    private readonly bool _isPooled;
    private bool _disposed;

    public Field(int width, int height) : this(width, height, new float[width * height], isPooled: false)
    {
        
    }

    private Field(int width, int height, float[] buffer, bool isPooled)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Width = width;
        Height = height;
        
        _length = width * height;
        _values = buffer;
        _isPooled = isPooled;
    }

    public int Width { get; }
    public int Height { get; }
    
    public Span<float> Values => _values.AsSpan(0, _length);

    public float this[int x, int y]
    {
        get => _values[Index(ClampX(x), ClampY(y))];
        set => _values[Index(ClampX(x), ClampY(y))] = Math.Clamp(value, 0f, 1f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetUnchecked(int x, int y) => _values[y * Width + x];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetUnchecked(int x, int y, float value) => _values[y * Width + x] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetUnchecked(int index) => _values[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetUnchecked(int index, float value) => _values[index] = value;

    public float Sample(float u, float v)
    {
        var x = Math.Clamp(u, 0f, 1f) * (Width - 1);
        var y = Math.Clamp(v, 0f, 1f) * (Height - 1);

        var x0 = (int)MathF.Floor(x);
        var y0 = (int)MathF.Floor(y);

        var x1 = Math.Min(Width - 1, x0 + 1);
        var y1 = Math.Min(Height - 1, y0 + 1);

        var tx = x - x0;
        var ty = y - y0;

        var a = this[x0, y0];
        var b = this[x1, y0];
        var c = this[x0, y1];
        var d = this[x1, y1];

        return Lerp(Lerp(a, b, tx), Lerp(c, d, tx), ty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private int Index(int x, int y) => y * Width + x;

    private int ClampX(int x) => Math.Clamp(x, 0, Width - 1);

    private int ClampY(int y) => Math.Clamp(y, 0, Height - 1);
    
    public static Field Rent(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        var buffer = ArrayPool<float>.Shared.Rent(width * height);
        
        Array.Clear(buffer, 0, width * height);

        return new Field(width, height, buffer, isPooled: true);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_isPooled) 
            ArrayPool<float>.Shared.Return(_values);
    }
}