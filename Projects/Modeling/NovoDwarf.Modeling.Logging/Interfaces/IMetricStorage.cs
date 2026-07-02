namespace NovoDwarf.Modeling.Logging.Interfaces;

public interface IMetricStorage
{
	void Store(IMetricEvent metric);
	IReadOnlyCollection<IMetricEvent> GetAll();
	void Clear();
}