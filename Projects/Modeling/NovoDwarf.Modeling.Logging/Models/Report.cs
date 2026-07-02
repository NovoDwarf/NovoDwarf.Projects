using NovoDwarf.Modeling.Logging.Interfaces;

namespace NovoDwarf.Modeling.Logging.Models;

public class Report
{
	public IReadOnlyCollection<IMetricEvent> Metrics;

	public Report(IReadOnlyCollection<IMetricEvent> metrics, DateTime dt)
	{
		Metrics = metrics;
	}
}