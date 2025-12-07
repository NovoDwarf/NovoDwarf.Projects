using Modeling.Logging.Models;

namespace Modeling.Logging.Interfaces;

public interface IMetricSink
{
	void Flush(Report report);
}