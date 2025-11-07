using Structures.Models.Abstracts.Commons.Options;

namespace Structures.Models.Abstracts.Options;

public class SourceOptions : DistributionOptions
{
	public bool ClosedSystem { get; private set; } = false;
	public int ClosedPopulation { get; private set; } = 1;
}