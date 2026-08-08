using System.IO;

namespace Grekov.Core;

public abstract partial class Def
{
	public DefId Id { get; set; }
	public string PackageId { get; internal set; } = string.Empty;
	public string ResourcePath { get; internal set; } = string.Empty;
}
