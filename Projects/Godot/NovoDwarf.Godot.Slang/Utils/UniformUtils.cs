using Godot;

namespace NovoDwarf.Godot.Slang.Utils;

internal static class UniformUtils
{
	public static RDUniform Make(Rid rid, int binding)
	{
		var uniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.StorageBuffer,
			Binding = binding
		};
		uniform.AddId(rid);
		return uniform;
	}
}
