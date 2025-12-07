using System.Text;
using Modeling.Logging.Interfaces;
using Modeling.Logging.Models;
using Modeling.Logging.Models.Events;

namespace Modeling.DeltaT.Algorithm.Sinks;

/*
 * TODO: rework this shit 'cause sink must be simple and generic
 * current realization is a mess
 *
 */

public class StatisticsSink : IMetricSink
{
	private Report _report;

	public void Flush(Report report)
	{
		var sb = new StringBuilder();

		_report = report;

		AppendHeader(sb);
		AppendSystemMetrics(sb);
		AppendNodeMetrics(sb);
		AppendFooter(sb);

		Console.WriteLine(sb.ToString());
	}

	private void AppendHeader(StringBuilder sb)
	{
		sb.AppendLine("╔════════════════════════════════════════════════════════════════╗");
		sb.AppendLine("║              СТАТИСТИКА СИМУЛЯЦИИ                              ║");
		sb.AppendLine("╚════════════════════════════════════════════════════════════════╝");
	}

	private void AppendFooter(StringBuilder sb)
	{
	}

	private void AppendSystemMetrics(StringBuilder sb)
	{
		var (totalTime, totalGenerated, totalCompleted) = CalculateSystemMetrics();

		sb.AppendLine("[Общие метрики системы]");
		sb.AppendLine($"-> Время симуляции: {totalTime:F3}");
		sb.AppendLine($"-> Всего заявок создано: {totalGenerated:F0}");
		sb.AppendLine($"-> Всего заявок завершено: {totalCompleted:F0}");
		sb.AppendLine();
	}

	private (double totalTime, double totalGenerated, double totalCompleted) CalculateSystemMetrics()
	{
		var totalTime = 0.0;
		var totalGenerated = 0.0;
		var totalCompleted = 0.0;

		foreach (var metric in _report.Metrics)
		{
			if (metric.Name != "System_TotalTime" || metric is not GaugeEvent timeGauge)
				continue;

			totalTime = timeGauge.Value;

			break;
		}

		var metricsByNode = GroupMetricsByNode();

		foreach (var nodeMetrics in metricsByNode.Values)
		{
			if (nodeMetrics.TryGetValue("Source_Generated", out var gen) && gen is CounterEvent genCounter)
				totalGenerated += genCounter.Value;

			if (nodeMetrics.TryGetValue("Sink_Completed", out var compl) && compl is CounterEvent complCounter)
				totalCompleted += complCounter.Value;
		}

		return (totalTime, totalGenerated, totalCompleted);
	}

	private Dictionary<string, Dictionary<string, IMetricEvent>> GroupMetricsByNode()
	{
		var metricsByNode = new Dictionary<string, Dictionary<string, IMetricEvent>>();

		foreach (var metric in _report.Metrics)
		{
			var parts = metric.Name.Split('_');

			if (parts.Length < 2)
				continue;

			var nodeId = parts[0];
			var metricType = string.Join('_', parts.Skip(1));

			if (!metricsByNode.ContainsKey(nodeId))
				metricsByNode[nodeId] = new Dictionary<string, IMetricEvent>();

			metricsByNode[nodeId][metricType] = metric;
		}

		return metricsByNode;
	}

	private void AppendNodeMetrics(StringBuilder sb)
	{
		var metricsByNode = GroupMetricsByNode();
		var totalTime = GetTotalSimulationTime(_report);

		foreach (var (nodeId, nodeMetrics) in metricsByNode.OrderBy(x => x.Key))
		{
			var nodeType = DetermineNodeType(nodeMetrics);
			AppendNodeHeader(sb, nodeId, nodeType);
			AppendServiceMetrics(sb, nodeMetrics, totalTime);
			AppendQueueMetrics(sb, nodeMetrics);
			AppendSourceMetrics(sb, nodeMetrics);
			AppendSinkMetrics(sb, nodeMetrics);
			sb.AppendLine();
		}
	}

	private string DetermineNodeType(Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (nodeMetrics.ContainsKey("Service_Completed"))
			return "Service";

		if (nodeMetrics.ContainsKey("Queue_Size_History") || nodeMetrics.ContainsKey("Queue_WaitTime") ||
		    nodeMetrics.ContainsKey("Queue_Dropped"))
			return "Queue";

		if (nodeMetrics.ContainsKey("Source_Generated"))
			return "Source";

		if (nodeMetrics.ContainsKey("Sink_Completed"))
			return "Sink";

		return "Empty";
	}

	private double GetTotalSimulationTime(Report report)
	{
		foreach (var metric in report.Metrics)
			if (metric.Name == "System_TotalTime" && metric is GaugeEvent timeGauge)
				return timeGauge.Value;

		return 0.0;
	}

	private void AppendNodeHeader(StringBuilder sb, string nodeId, string nodeType)
	{
		sb.AppendLine($"[Узел] [{nodeType}]: [{nodeId}]");
	}

	private void AppendServiceMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics, double totalTime)
	{
		if (!nodeMetrics.TryGetValue("Service_Completed", out var serviceCompleted) ||
		    serviceCompleted is not CounterEvent completedCounter)
			return;

		var count = completedCounter.Value;
		sb.AppendLine($"-> Обработано заявок: {count:F0}");

		AppendDurationMetrics(sb, nodeMetrics);
		AppendUtilizationMetrics(sb, nodeMetrics, totalTime);
	}

	private void AppendDurationMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (!nodeMetrics.TryGetValue("Service_Duration", out var duration) ||
		    duration is not NumericListEvent { Count: > 0 } durationList)
			return;

		sb.AppendLine("[Время обработки]");
		sb.AppendLine($"-> Среднее: {durationList.Average:F3}");
		sb.AppendLine($"-> Минимальное: {durationList.Min:F3}");
		sb.AppendLine($"-> Максимальное: {durationList.Max:F3}");
	}

	private void AppendUtilizationMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics,
		double totalTime)
	{
		if (!nodeMetrics.TryGetValue("Service_BusyTime", out var busyTime) || busyTime is not NumericListEvent busyList)
			return;

		var totalBusyTime = busyList.Sum;

		if (!(totalBusyTime > 0) || !(totalTime > 0))
			return;

		var utilization = totalBusyTime / totalTime * 100;
		sb.AppendLine($"-> Утилизация: {utilization:F2}%");
		sb.AppendLine($"-> Суммарное время занятости: {totalBusyTime:F3}");
	}

	private void AppendQueueMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		AppendQueueSizeMetrics(sb, nodeMetrics);
		AppendWaitTimeMetrics(sb, nodeMetrics);
	}

	private void AppendQueueSizeMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (!nodeMetrics.TryGetValue("Queue_Size_History", out var queueSizeHistory) ||
		    queueSizeHistory is not NumericListEvent { Count: > 0 } sizeList)
			return;

		sb.AppendLine("[Размер очереди]");
		sb.AppendLine($"-> Среднее: {sizeList.Average:F3}");
		sb.AppendLine($"-> Минимальное: {sizeList.Min:F3}");
		sb.AppendLine($"-> Максимальное: {sizeList.Max:F3}");
	}

	private void AppendWaitTimeMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (!nodeMetrics.TryGetValue("Queue_WaitTime", out var waitTime) ||
		    waitTime is not NumericListEvent { Count: > 0 } waitList)
			return;

		sb.AppendLine("[Время ожидания в очереди]");
		sb.AppendLine($"-> Среднее: {waitList.Average:F3}");
		sb.AppendLine($"-> Минимальное: {waitList.Min:F3}");
		sb.AppendLine($"-> Максимальное: {waitList.Max:F3}");
	}

	private void AppendSourceMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		AppendGeneratedMetrics(sb, nodeMetrics);
		AppendGenerationTimeMetrics(sb, nodeMetrics);
		AppendBlockTimeMetrics(sb, nodeMetrics);
	}

	private void AppendGeneratedMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (nodeMetrics.TryGetValue("Source_Generated", out var generated) && generated is CounterEvent genCounter)
			sb.AppendLine($"-> Сгенерировано заявок: {genCounter.Value:F0}");
	}

	private void AppendGenerationTimeMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (!nodeMetrics.TryGetValue("Source_GenerationTime", out var genTime) ||
		    genTime is not NumericListEvent { Count: > 0 } genTimeList) return;

		sb.AppendLine("[Время генерации]");
		sb.AppendLine($"-> Минимальное: {genTimeList.Min:F3}");
		sb.AppendLine($"-> Среднее: {genTimeList.Average:F3}");
		sb.AppendLine($"-> Максимальное: {genTimeList.Max:F3}");
	}

	private void AppendBlockTimeMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (!nodeMetrics.TryGetValue("Source_BlockTime", out var blockTime) ||
		    blockTime is not NumericListEvent { Count: > 0 } blockList) return;

		sb.AppendLine("[Время простоя]");
		sb.AppendLine($"-> Среднее: {blockList.Average:F3}");
		sb.AppendLine($"-> Минимальное: {blockList.Min:F3}");
		sb.AppendLine($"-> Максимальное: {blockList.Max:F3}");
		sb.AppendLine($"-> Количество простоев: {blockList.Count}");
	}

	private void AppendSinkMetrics(StringBuilder sb, Dictionary<string, IMetricEvent> nodeMetrics)
	{
		if (nodeMetrics.TryGetValue("Sink_Completed", out var completed) && completed is CounterEvent complCounter)
			sb.AppendLine($"-> Завершено заявок: {complCounter.Value:F0}");
	}
}