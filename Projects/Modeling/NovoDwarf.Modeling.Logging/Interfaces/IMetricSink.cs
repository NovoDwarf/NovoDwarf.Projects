using NovoDwarf.Modeling.Logging.Models;

namespace NovoDwarf.Modeling.Logging.Interfaces;

public interface IMetricSink
{
	void Flush(Report report);
}