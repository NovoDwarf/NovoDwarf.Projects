using NovoDwarf.Modeling.Logging.Interfaces;
using NovoDwarf.Modeling.Logging.Models;

namespace NovoDwarf.Modeling.Logging.Sinks;

public class ConsoleSink : IMetricSink
{
	public void Flush(Report report)
	{
		Console.WriteLine(report);
	}
}