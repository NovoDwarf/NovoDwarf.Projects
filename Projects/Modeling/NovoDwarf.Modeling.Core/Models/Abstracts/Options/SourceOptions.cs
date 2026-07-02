using Modeling.Core.Models.Abstracts.Commons.Options;

namespace Modeling.Core.Models.Abstracts.Options;

public class SourceOptions : DistributionOptions
{
	public bool ClosedSystem { get; set; } = false;
	public int ClosedPopulation { get; set; } = 1;
}