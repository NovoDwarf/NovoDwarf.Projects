using Logging.Interfaces;
using Logging.Models;

namespace Logging.Sinks;

public class ConsoleSink : IMetricSink
{
	public void Flush(Report report)
	{
		Console.WriteLine(report);
	}
}