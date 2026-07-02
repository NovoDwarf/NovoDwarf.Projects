using NovoDwarf.Modeling.Logging.Models;
using NovoDwarf.Modeling.Logging.Sinks;

namespace NovoDwarf.Modeling.Logging.Ext;

public static class MetricCollectorExtensions
{
	extension(MetricCollector collector)
	{
		public void WithConsole()
		{
			var sink = new ConsoleSink();

			collector.AddSink(sink);
		}
	}
}