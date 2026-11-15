using NovoDwarf.Rml;
using Brutal.GlfwApi;
using Brutal.Numerics;

namespace NovoDwarf.Brutal.Rml;

public sealed class RmlGlfwInput : IDisposable
{
    private readonly GlfwWindow _window;
    private readonly RmlContext _ui;
    
    private readonly Queue<Action> _events = [];
    private readonly Func<string>? _previousReadClipboard;
    private readonly Action<string>? _previousWriteClipboard;
    
    public RmlGlfwInput(GlfwWindow window, RmlContext ui)
    {
        _window = window;
        _ui = ui;
       
        _previousReadClipboard = ui.RenderData.ReadClipboard;
        _previousWriteClipboard = ui.RenderData.WriteClipboard;
        
        ui.RenderData.ReadClipboard = () => Glfw.Clipboard ?? string.Empty;
        ui.RenderData.WriteClipboard = Glfw.SetClipboard;
      
        window.OnMouseButton += OnButton;
        window.OnScroll += OnScroll;
        window.OnKey += OnKey;
        window.OnChar += OnChar;
    }

    public int Width { get; private set; } = 1280;
    public int Height { get; private set; } = 800;
    public float Scale { get; private set; } = 1;
    public float UiScale { get; set; } = 1;
    public bool CapturedMouse { get; private set; }

    private bool _disposed;
    private bool _draggingUi;
    private int _modifiers;
    
    public void Update()
    {
        var framebuffer = _window.FramebufferSize;
        var size = _window.Size;
       
        Width = Math.Max(1, framebuffer.X);
        Height = Math.Max(1, framebuffer.Y);
        Scale = (size.X > 0 ? (float)Width / size.X : 1) * UiScale;
        CapturedMouse = _draggingUi;
        
        _ui.Update( Width, Height, Scale);
       
        if (!_window.IsFocused)
        {
            _events.Clear();
            _draggingUi = false;
            _modifiers = 0;
           
            _ui.ProcessMouseLeave();
            _ui.ClearFocus();
            
            return;
        }
        
        while (_events.TryDequeue(out var action))
            action();
       
        Move(_window.CursorPos);
    }

    private void Move(double2 cursor)
    {
        var size = _window.Size;
        
        if (size is { X: > 0, Y: > 0 })
            _ui.ProcessMouseMove( (int)(cursor.X * Width / size.X), (int)(cursor.Y * Height / size.Y), _modifiers);
    }

    private void OnButton(GlfwWindow window, GlfwMouseButton button, GlfwButtonAction action, GlfwModifier modifiers)
    {
        var cursor = window.CursorPos;
       
        _events.Enqueue(() =>
        {
            Move(cursor);
            
            _modifiers = Modifiers(modifiers);
           
            var overUi = _ui.CapturesMouse;
           
            if (action == GlfwButtonAction.Press)
            {
                if (!overUi)
                    _ui.ClearFocus();
                _draggingUi = overUi;
            }
            
            CapturedMouse |= overUi || _draggingUi;
           
            _ui.ProcessMouseButton( (int)button, action == GlfwButtonAction.Press, _modifiers);
         
            if (action != GlfwButtonAction.Press)
                _draggingUi = false;
        });
    }

    private void OnScroll(GlfwWindow window, double2 offset)
    {
        var cursor = window.CursorPos;
       
        _events.Enqueue(() =>
        {
            Move(cursor);
            CapturedMouse |= _ui.CapturesMouse;
            _ui.ProcessWheel( -(float)offset.X, -(float)offset.Y, _modifiers);
        });
    }

    private void OnKey(GlfwWindow window, GlfwKey key, int scanCode, GlfwKeyAction action, GlfwModifier modifiers)
    {
        _events.Enqueue(() =>
        {
            _modifiers = Modifiers(modifiers);
           
            var identifier = KeyIdentifier((int)key);
            
            if (identifier != 0)
                _ui.ProcessKey( identifier, action != GlfwKeyAction.Release, _modifiers);
        });
    }

    private void OnChar(GlfwWindow window, uint codepoint)
    {
        _events.Enqueue(() => _ui.ProcessText( char.ConvertFromUtf32((int)codepoint)));
    }

    public static int Modifiers(GlfwModifier value)
    {
        var flags = (int)value;
        
        return ((flags & 2) != 0 ? 1 : 0) | ((flags & 1) != 0 ? 2 : 0) |
               ((flags & 4) != 0 ? 4 : 0) | ((flags & 8) != 0 ? 8 : 0) |
               ((flags & 16) != 0 ? 16 : 0) | ((flags & 32) != 0 ? 32 : 0);
    }
    
    public static int KeyIdentifier(int key) => key switch
    {
        >= 65 and <= 90 => key - 65 + 12,    // A-Z
        >= 48 and <= 57 => key - 48 + 2,     // 0-9
        >= 290 and <= 313 => key - 290 + 107, // F1-F24
        >= 320 and <= 329 => key - 320 + 51,  // Numpad 0-9

        // Navigation
        256 => 81, // Escape
        257 => 72, // Enter
        258 => 70, // Tab
        259 => 69, // Backspace
        260 => 98, // Insert
        261 => 99, // Delete
        262 => 92, // Right
        263 => 90, // Left
        264 => 93, // Down
        265 => 91, // Up
        266 => 86, // Page Up
        267 => 87, // Page Down
        268 => 89, // Home
        269 => 88, // End

        // Modifiers
        340 => 138, // Left Shift
        344 => 139, // Right Shift
        341 => 140, // Left Control
        345 => 141, // Right Control
        342 => 142, // Left Alt
        346 => 143, // Right Alt
        343 => 175, // Left Super
        347 => 176, // Right Super

        // Numpad
        335 => 61, // Enter
        330 => 66, // Decimal
        331 => 67, // Divide
        332 => 62, // Multiply
        333 => 65, // Subtract
        334 => 63, // Add

        // Other
        32 => 1,   // Space
        45 => 41,  // Minus
        61 => 39,  // Equal
        44 => 40,  // Comma
        46 => 42,  // Period
        47 => 43,  // Slash
        59 => 38,  // Semicolon
        39 => 48,  // Apostrophe
        91 => 45,  // Left Bracket
        92 => 46,  // Backslash
        93 => 47,  // Right Bracket
        96 => 44,  // Grave Accent

        _ => 0
    };
    
    public void Clear()
    {
        _events.Clear();
        _draggingUi = false;
        
        CapturedMouse = false;
        
        _modifiers = 0;
        
        _ui.ProcessMouseLeave();
       
        for (var button = 0; button < 3; button++)
            _ui.ProcessMouseButton( button, false, 0);
       
        _ui.ClearFocus();
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;
      
        _disposed = true;
        
        _ui.RenderData.ReadClipboard = _previousReadClipboard;
        _ui.RenderData.WriteClipboard = _previousWriteClipboard;
        
        _window.OnMouseButton -= OnButton;
        _window.OnScroll -= OnScroll;
        _window.OnKey -= OnKey;
        _window.OnChar -= OnChar;
       
        _events.Clear();
    }
}



