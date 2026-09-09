using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Characters;

public class DynastyDef : Def
{
	[DefField]
	public string FamilyName { get; set; } = string.Empty;
	
	public Guid? FounderId { get; set; }
}
