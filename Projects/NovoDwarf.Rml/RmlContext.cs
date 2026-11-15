using System.Runtime.Versioning;
using System.Text;

namespace NovoDwarf.Rml;

public sealed unsafe class RmlContext : IDisposable
{
    private readonly nint _native;

    [SupportedOSPlatform("Windows")]
    public RmlContext(string fontPath, string documentPath)
    {
        if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
            throw new PlatformNotSupportedException("The bundled RmlUi runtime currently supports Windows x64.");

        ArgumentException.ThrowIfNullOrWhiteSpace(fontPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentPath);

        var callbacks = RenderData.Activate();

        try
        {
            _native = RmlNative.Create(callbacks, fontPath, documentPath);
            RenderData.ThrowIfFailed();

            if (_native == 0)
                throw new InvalidOperationException($"Could not initialize RmlUi. {string.Join(Environment.NewLine, RenderData.Messages)}");
        }
        catch
        {
            if (_native != 0)
                RmlNative.Destroy(_native);

            RenderData.Deactivate();
            throw;
        }
    }

    public RmlRenderData RenderData { get; } = new();

    public bool CapturesMouse => RmlNative.MouseCaptured(Handle) != 0;
    public bool CapturesKeyboard => RmlNative.KeyboardCaptured(Handle) != 0;

    private nint Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _native;
        }
    }

    private bool _disposed;

    public void BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RenderData.BeginFrame();
    }

    public void Update(int width, int height, float scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        if (!float.IsFinite(scale) || scale <= 0)
            throw new ArgumentOutOfRangeException(nameof(scale));

        RmlNative.Update(Handle, width, height, scale);
        RenderData.ThrowIfFailed();
    }

    public void Render()
    {
        RmlNative.Render(Handle);
        RenderData.ThrowIfFailed();
    }

    public void SetMarkup(string id, string markup)
    {
        RmlNative.SetRml(Handle, id, markup);
    }

    public void SetProperty(string id, string name, string value)
    {
        RmlNative.SetProperty(Handle, id, name, value);
    }

    public void SetAttribute(string id, string name, string? value)
    {
        RmlNative.SetAttribute(Handle, id, name, value);
    }

    public bool IsChecked(string id)
    {
        return RmlNative.Checked(Handle, id) != 0;
    }

    public bool Click(string id)
    {
        return RmlNative.Click(Handle, id) != 0;
    }

    public int ProcessMouseMove(int x, int y, int modifiers = 0)
    {
        return RmlNative.MouseMove(Handle, x, y, modifiers);
    }

    public int ProcessMouseButton(int button, bool down, int modifiers = 0)
    {
        return RmlNative.MouseButton(Handle, button, down ? 1 : 0, modifiers);
    }

    public void ProcessMouseLeave()
    {
        RmlNative.MouseLeave(Handle);
    }

    public void ClearFocus()
    {
        RmlNative.Blur(Handle);
    }

    public void ProcessWheel(float x, float y, int modifiers = 0)
    {
        RmlNative.Wheel(Handle, x, y, modifiers);
    }

    public void ProcessKey(int key, bool down, int modifiers = 0)
    {
        RmlNative.Key(Handle, key, down ? 1 : 0, modifiers);
    }

    public void ProcessText(string text)
    {
        RmlNative.Text(Handle, text);
    }
    
    public string GetValue(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        Span<byte> buffer = stackalloc byte[512];

        fixed (byte* ptr = buffer)
        {
            var length = RmlNative.GetValue(Handle, id, ptr, buffer.Length);

            if (length < 0)
                throw new InvalidOperationException("Failed to retrieve element value.");

            if (length < buffer.Length)
                return Encoding.UTF8.GetString(buffer[..length]);

            var large = new byte[checked(length + 1)];

            fixed (byte* largePtr = large)
            {
                var actual = RmlNative.GetValue(Handle, id, largePtr, large.Length);

                if (actual != length)
                    throw new InvalidOperationException("Element value changed during retrieval.");
            }

            return Encoding.UTF8.GetString(large.AsSpan(0, length));
        }
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            RmlNative.Destroy(_native);
        }
        finally
        {
            RenderData.Deactivate();
        }
    }
}
