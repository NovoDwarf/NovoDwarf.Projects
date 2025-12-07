using Modeling.Logging.Models;
using Modeling.Logging.Sinks;

namespace Modeling.Logging.Ext;

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