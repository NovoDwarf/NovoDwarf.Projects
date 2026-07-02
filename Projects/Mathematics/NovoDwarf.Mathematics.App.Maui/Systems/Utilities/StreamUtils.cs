using System.Text;

namespace NovoDwarf.Mathematics.App.Systems.Utilities;

public static class StreamUtils
{
	public static void WriteString(Stream stream, string s)
	{
		var bytes = Encoding.ASCII.GetBytes(s);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteInt(Stream stream, int value)
	{
		var bytes = BitConverter.GetBytes(value);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteShort(Stream stream, short value)
	{
		var bytes = BitConverter.GetBytes(value);
		stream.Write(bytes, 0, bytes.Length);
	}
}