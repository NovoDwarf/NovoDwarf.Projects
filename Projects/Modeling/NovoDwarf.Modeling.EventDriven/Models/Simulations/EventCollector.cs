using Modeling.Core.EX;

namespace NovoDwarf.Modeling.EventDriven.Models.Simulations;

/// <summary>
///     Обёртка над обработчиком события симуляции.
///     Держит время наступления события и делегат, который его обрабатывает.
/// </summary>
public sealed class SimulationEvent
{
	public SimulationEvent(double timestamp, Action<SimulationContext, EventCollector> handler)
	{
		Timestamp = timestamp;
		_handler = handler;
	}

	public double Timestamp { get; }

	private readonly Action<SimulationContext, EventCollector> _handler;

	public void Execute(SimulationContext context, EventCollector collector)
	{
		_handler(context, collector);
	}
}

/// <summary>
///     Общий коллектора событий для событийно-ориентированной модели.
///     Хранит события в приоритетной очереди по времени наступления.
/// </summary>
public sealed class EventCollector
{
	private readonly PriorityQueue<SimulationEvent, double> _queue = new();


	public int Count => _queue.Count;

	public void Enqueue(SimulationEvent ev)
	{
		_queue.Enqueue(ev, ev.Timestamp);
	}

	public bool TryDequeue(out SimulationEvent? ev)
	{
		if (_queue.Count == 0)
		{
			ev = null;
			return false;
		}

		ev = _queue.Dequeue();
		return true;
	}
}


