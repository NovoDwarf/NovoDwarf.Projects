using System.Runtime.InteropServices;

namespace NovoDwarf.Rml;

[StructLayout(LayoutKind.Sequential)]
public struct RmlVertex
{
    public float X;
    public float Y;
    public uint Color;
    public float U;
    public float V;
}
