namespace Komissar.Diagnostics;

public sealed record TickMetrics(long Tick, TimeSpan Delta, TimeSpan Duration, int SystemCount, int WaveCount);
