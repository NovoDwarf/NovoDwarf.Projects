using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Common;

public class LocationDef : Def
{
	[DefField]
	public string Region { get; set; } = string.Empty;
}
