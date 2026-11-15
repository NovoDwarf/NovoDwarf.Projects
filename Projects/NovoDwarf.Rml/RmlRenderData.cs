using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace NovoDwarf.Rml;

public sealed unsafe class RmlRenderData
{
    public sealed record Geometry(RmlVertex[] Vertices, int[] Indices);

    public sealed record Texture(byte[] Pixels, int Width, int Height);

    public readonly record struct Draw(
        Geometry Geometry,
        float X,
        float Y,
        nuint Texture,
        Matrix4x4 Transform,
        int XClip,
        int YClip,
        int WidthClip,
        int HeightClip);

    private static RmlRenderData? _active;

    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly Dictionary<nuint, Geometry> _geometry = [];
    private readonly Queue<string> _actions = [];

    private readonly List<Draw> _draws = [];
    private readonly Dictionary<nuint, Texture> _textures = [];
    private readonly List<string> _messages = [];

    public IReadOnlyList<Draw> Draws => _draws;
    public IReadOnlyDictionary<nuint, Texture> Textures => _textures;
    public IReadOnlyList<string> Messages => _messages;

    public int VertexCount => Draws.Sum(draw => draw.Geometry.Vertices.Length);
    public int IndexCount => Draws.Sum(draw => draw.Geometry.Indices.Length);

    public Func<string>? ReadClipboard { get; set; }
    public Action<string>? WriteClipboard { get; set; }

    private ExceptionDispatchInfo? _failure;
    private Matrix4x4 _transform = Matrix4x4.Identity;
    private nuint _nextHandle = 1;
    private (int X, int Y, int Width, int Height) _clip = (0, 0, -1, -1);

    internal RmlNative.Callbacks Activate()
    {
        if (Interlocked.CompareExchange(ref _active, this, null) is not null)
            throw new InvalidOperationException("Only one RmlUi context may be active in this process.");

        return new RmlNative.Callbacks
        {
            Compile = &Compile,
            Draw = &Render,
            ReleaseGeometry = &ReleaseGeometry,
            Texture = &GenerateTexture,
            ReleaseTexture = &ReleaseTexture,
            Scissor = &Scissor,
            Transform = &Transform,
            Time = &Time,
            Log = &Log,
            Action = &Action,
            ClipboardRead = &ClipboardRead,
            ClipboardWrite = &ClipboardWrite
        };
    }

    internal void Deactivate()
    {
        Interlocked.CompareExchange(ref _active, null, this);

        _draws.Clear();
        _geometry.Clear();
        _textures.Clear();
        _actions.Clear();
        ReadClipboard = null;
        WriteClipboard = null;
    }

    internal void BeginFrame()
    {
        ThrowIfFailed();

        _draws.Clear();
        _clip = (0, 0, -1, -1);
        _transform = Matrix4x4.Identity;
    }

    public bool TryDequeueAction(out string action)
    {
        return _actions.TryDequeue(out action!);
    }

    public void ThrowIfFailed()
    {
        _failure?.Throw();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static nuint Compile(RmlVertex* vertices, int vertexCount, int* indices, int indexCount)
    {
        try
        {
            var active = _active!;
            var handle = active._nextHandle++;

            active._geometry.Add(handle, new Geometry(
                    new ReadOnlySpan<RmlVertex>(vertices, vertexCount).ToArray(),
                    new ReadOnlySpan<int>(indices, indexCount).ToArray()));

            return handle;
        }
        catch (Exception error)
        {
            Fail(error);
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Render(nuint geometry, float x, float y, nuint texture)
    {
        try
        {
            var active = _active!;
            var clip = active._clip;

            active._draws.Add(new Draw(
                active._geometry[geometry],
                x, y,
                texture, active._transform,
                clip.X, clip.Y,
                clip.Width, clip.Height));
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void ReleaseGeometry(nuint geometry)
    {
        try
        {
            _active!._geometry.Remove(geometry);
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static nuint GenerateTexture(byte* pixels, int width, int height)
    {
        try
        {
            var active = _active!;
            var handle = active._nextHandle++;

            active._textures.Add(
                handle,
                new Texture(new ReadOnlySpan<byte>(pixels, checked(width * height * 4)).ToArray(), width, height));

            return handle;
        }
        catch (Exception error)
        {
            Fail(error);
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void ReleaseTexture(nuint texture)
    {
        try
        {
            _active!._textures.Remove(texture);
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Scissor(int x, int y, int width, int height)
    {
        if (_active is { } active)
            active._clip = (x, y, width, height);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Transform(float* matrix)
    {
        if (_active is { } active)
            active._transform = matrix == null ? Matrix4x4.Identity : *(Matrix4x4*)matrix;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static double Time()
    {
        return _active?._clock.Elapsed.TotalSeconds ?? 0;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Log(int level, byte* message)
    {
        try
        {
            _active?._messages.Add($"RmlUi [{level}]: {Marshal.PtrToStringUTF8((nint)message)}");
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Action(byte* action)
    {
        try
        {
            _active?._actions.Enqueue(Marshal.PtrToStringUTF8((nint)action) ?? string.Empty);
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int ClipboardRead(byte* buffer, int capacity)
    {
        try
        {
            var value = _active?.ReadClipboard?.Invoke() ?? string.Empty;
            var length = Encoding.UTF8.GetByteCount(value);

            if (buffer == null || capacity <= length)
                return length;

            Encoding.UTF8.GetBytes(value, new Span<byte>(buffer, capacity));

            buffer[length] = 0;
            return length;
        }
        catch (Exception error)
        {
            Fail(error);
            return -1;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void ClipboardWrite(byte* text)
    {
        try
        {
            _active?.WriteClipboard?.Invoke(Marshal.PtrToStringUTF8((nint)text) ?? string.Empty);
        }
        catch (Exception error)
        {
            Fail(error);
        }
    }
    
    private static void Fail(Exception error)
    {
        if (_active is { } active)
            active._failure ??= ExceptionDispatchInfo.Capture(error);
    }
}
