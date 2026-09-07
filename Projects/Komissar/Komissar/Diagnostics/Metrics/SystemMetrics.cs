namespace Komissar.Diagnostics;

public sealed record SystemMetrics(string SystemName, long Tick, TimeSpan Duration, int Wave);