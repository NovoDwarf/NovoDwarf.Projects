using Modeling.Logging.Interfaces;
using Modeling.Logging.Models;

namespace Modeling.Logging.Sinks;

public class FileSink : IMetricSink
{
	private readonly string _path;

	public FileSink(string path)
	{
		_path = path;
	}

	public void Flush(Report report)
	{
		File.AppendAllText(_path, report + Environment.NewLine);
	}
}