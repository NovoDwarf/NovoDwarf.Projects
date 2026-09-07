using Grekov.Core.Attributes;

namespace Grekov.Core;

public abstract class Def
{
	public DefId Id { get; set; }
	
	[DefField]
	public string Name { get; set; } = string.Empty;
	public string Description => Name + "_desc";
	public string Full => Name + "_full";
	public string Image => Name + "_image";
	
	public string PackageId { get; internal set; } = string.Empty;
	public string ResourcePath { get; internal set; } = string.Empty;
	public string ResourceName => Path.GetFileNameWithoutExtension(ResourcePath);
}
