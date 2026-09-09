using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Characters;

public class EthnicityDef : Def
{
	[DefField]
	public List<string> FemaleNames { get; set; } = [];
	
	[DefField]
	public List<string> MaleNames { get; set; } = [];
	
	[DefField]
	public List<string> FamilyNames { get; set; } = [];
}
