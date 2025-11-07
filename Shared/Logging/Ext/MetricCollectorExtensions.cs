using Logging.Models;
using Logging.Sinks;

namespace Logging.Ext;

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