namespace Grekov;

public abstract partial class Def
{
	public DefId Id { get; set; }
	public string PackageId { get; internal set; } = string.Empty;
	public string ResourcePath { get; internal set; } = string.Empty;
	public string ResourceName => Path.GetFileNameWithoutExtension(ResourcePath);
}
