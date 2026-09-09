using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Common;

public class LanguageDef : Def
{
	[DefField]
	public string Code { get; set; } = string.Empty;
}
