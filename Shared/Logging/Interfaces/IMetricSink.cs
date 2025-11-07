using Logging.Models;

namespace Logging.Interfaces;

public interface IMetricSink
{
	void Flush(Report report);
}