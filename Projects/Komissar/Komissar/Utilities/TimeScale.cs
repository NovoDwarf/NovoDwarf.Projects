namespace Komissar.Utilities;

public sealed class TimeScale
{
    public double Speed { get; set; } = 1;

    public bool IsPaused { get; set; }

    public TimeSpan Convert(TimeSpan realDelta)
    {
        if (IsPaused)
            return TimeSpan.Zero;
        
        return TimeSpan.FromTicks((long)(realDelta.Ticks * Speed));
    }
}
