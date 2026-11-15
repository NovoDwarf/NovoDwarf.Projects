using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NovoDwarf.Rml;

internal static unsafe partial class RmlNative
{
    private const string Library = "NovoDwarf.Rml.Native";

    [StructLayout(LayoutKind.Sequential)]
    internal struct Callbacks
    {
        public delegate* unmanaged[Cdecl]<RmlVertex*, int, int*, int, nuint> Compile;
        public delegate* unmanaged[Cdecl]<nuint, float, float, nuint, void> Draw;
        public delegate* unmanaged[Cdecl]<nuint, void> ReleaseGeometry;
        public delegate* unmanaged[Cdecl]<byte*, int, int, nuint> Texture;
        public delegate* unmanaged[Cdecl]<nuint, void> ReleaseTexture;
        public delegate* unmanaged[Cdecl]<int, int, int, int, void> Scissor;
        public delegate* unmanaged[Cdecl]<float*, void> Transform;
        public delegate* unmanaged[Cdecl]<double> Time;
        public delegate* unmanaged[Cdecl]<int, byte*, void> Log;
        public delegate* unmanaged[Cdecl]<byte*, void> Action;
        public delegate* unmanaged[Cdecl]<byte*, int, int> ClipboardRead;
        public delegate* unmanaged[Cdecl]<byte*, void> ClipboardWrite;
    }

    [LibraryImport(Library, EntryPoint = "nd_rml_create", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint Create(Callbacks callbacks, string font, string document);

    [LibraryImport(Library, EntryPoint = "nd_rml_destroy", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Destroy(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_update", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Update(nint ui, int width, int height, float scale);

    [LibraryImport(Library, EntryPoint = "nd_rml_render", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Render(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_mouse_move", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int MouseMove(nint ui, int x, int y, int modifiers);

    [LibraryImport(Library, EntryPoint = "nd_rml_mouse_button", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int MouseButton(nint ui, int button, int down, int modifiers);

    [LibraryImport(Library, EntryPoint = "nd_rml_mouse_leave", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void MouseLeave(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_blur", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Blur(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_wheel", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Wheel(nint ui, float x, float y, int modifiers);

    [LibraryImport(Library, EntryPoint = "nd_rml_key", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Key(nint ui, int key, int down, int modifiers);

    [LibraryImport(Library, EntryPoint = "nd_rml_text", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Text(nint ui, string text);

    [LibraryImport(Library, EntryPoint = "nd_rml_mouse_captured", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int MouseCaptured(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_keyboard_captured", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int KeyboardCaptured(nint ui);

    [LibraryImport(Library, EntryPoint = "nd_rml_set_rml", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetRml(nint ui, string id, string rml);

    [LibraryImport(Library, EntryPoint = "nd_rml_set_property", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetProperty(nint ui, string id, string name, string value);

    [LibraryImport(Library, EntryPoint = "nd_rml_set_attribute", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetAttribute(nint ui, string id, string name, string? value);

    [LibraryImport(Library, EntryPoint = "nd_rml_get_value", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int GetValue(nint ui, string id, byte* buffer, int capacity);

    [LibraryImport(Library, EntryPoint = "nd_rml_checked", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Checked(nint ui, string id);

    [LibraryImport(Library, EntryPoint = "nd_rml_click", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Click(nint ui, string id);
}
