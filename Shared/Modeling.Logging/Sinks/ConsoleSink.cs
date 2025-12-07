using Modeling.Logging.Interfaces;
using Modeling.Logging.Models;

namespace Modeling.Logging.Sinks;

public class ConsoleSink : IMetricSink
{
	public void Flush(Report report)
	{
		Console.WriteLine(report);
	}
}