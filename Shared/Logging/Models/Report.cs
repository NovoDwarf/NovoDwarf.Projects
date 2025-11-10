using System;
using System.Collections.Generic;
using Logging.Interfaces;

namespace Logging.Models;

public class Report
{
	public IReadOnlyCollection<IMetricEvent> Metrics;

	public Report(IReadOnlyCollection<IMetricEvent> metrics, DateTime dt)
	{
		Metrics = metrics;
	}
}

/*using System.Text;
using Logging.Utilities;

namespace Logging.Models;

public class Report
{
public double Time { get; set; }

public double Requests { get; set; }
public double RequestsCompleted { get; set; }

public List<NodeStats> Nodes { get; set; } = [];

public override string ToString()
{
	var sb = new StringBuilder();

	sb.AppendLine("==== Общие метрики симуляции ====");
	sb.AppendLine($"Время: [{Time:F3}]");
	sb.AppendLine($"Создано: [{Requests}]");
	sb.AppendLine($"Завершено: [{RequestsCompleted}]");

	sb.AppendLine("==== Метрики по узлам ====");
	foreach (var s in Nodes.OrderBy(x => x.Name))
	{
		var nodeSb = new StringBuilder();

		nodeSb.AppendIfNonZero("обработано", s.Processed);
		nodeSb.AppendIfNonZero("пропущено", s.Dropped);
		nodeSb.AppendIfNonZero("зашло", s.Enqueued);
		nodeSb.AppendIfNonZero("вышло", s.Dequeued);
		nodeSb.AppendIfNonZero("обслужено", s.Services);
		nodeSb.AppendIfNonZero("тиков", s.Ticks);

		var util = GetUtilization(s, Time);
		if (util > 0) nodeSb.Append($"утилизация = [{util:F3}%], ");

		if (s.QueueWait.Count > 0)
			nodeSb.Append($"ожидание: ср=[{s.QueueWait.Average:F3}], мин=[{s.QueueWait.Min:F3}], макс=[{s.QueueWait.Max:F3}], ");

		if (s.GeneratorBlock.Count > 0)
			nodeSb.Append($"блокировка: ср=[{s.GeneratorBlock.Average:F3}], мин=[{s.GeneratorBlock.Min:F3}], макс=[{s.GeneratorBlock.Max:F3}], блокировок=[{s.GeneratorBlock.Count}], ");

		if (s.ServiceTime.Count > 0)
			nodeSb.Append($"обслуживание: ср=[{s.ServiceTime.Average:F3}], мин=[{s.ServiceTime.Min:F3}], макс=[{s.ServiceTime.Max:F3}], ");

		if (s.QueueLength.Count > 0)
			nodeSb.Append($"длина очереди: ср=[{s.QueueLength.Average:F3}], мин=[{s.QueueLength.Min:F3}], макс=[{s.QueueLength.Max:F3}], ");

		if (nodeSb.Length <= 0)
			continue;

		nodeSb.Length -= 2;
		sb.AppendLine($"Узел [{s.Name}]: {nodeSb}");
	}

	return sb.ToString();
}

private static double GetUtilization(NodeStats s, double totalTime)
{
	if (totalTime <= 0 || s.Services == 0)
		return 0;

	return (s.BusyTime / totalTime) * 100;
}
}*/