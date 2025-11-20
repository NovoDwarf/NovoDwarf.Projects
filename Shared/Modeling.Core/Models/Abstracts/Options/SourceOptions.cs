using Modeling.Core.Models.Abstracts.Commons.Options;

namespace Modeling.Core.Models.Abstracts.Options;

public class SourceOptions : DistributionOptions
{
	public bool ClosedSystem { get; private set; } = false;
	public int ClosedPopulation { get; private set; } = 1;
}