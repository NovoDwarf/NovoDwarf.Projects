using SultanDynasty.Definitons.Characters;
using SultanDynasty.Definitons.Common;
using SultanDynasty.Enums;

namespace SultanDynasty.Instances.Characters;

public class Identity
{
	public string FirstName { get; set; } = string.Empty;
	public string MiddleName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	
	public string FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

	public Gender Gender { get; set; }

	public DateOnly BirthDate { get; set; }
	public LocationDef BirthLocation { get; set; } = new();

	public TitleDef CurrentTitle { get; set; } = new();
	public List<LanguageDef> KnownLanguages { get; set; } = [];
	public EthnicityDef Ethnicity { get; set; } = new();
	public ReligionDef Religion { get; set; } = new();
	public DynastyDef Dynasty { get; set; } = new();

	public Guid? FatherId { get; set; }
	public Guid? MotherId { get; set; }
}
