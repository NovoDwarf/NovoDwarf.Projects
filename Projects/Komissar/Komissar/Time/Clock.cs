namespace Komissar.Time;

public sealed class Clock
{
    public TimeSpan Elapsed { get; private set; }

    public void Advance(TimeSpan delta)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(delta, TimeSpan.Zero);

        Elapsed += delta;
    }
}
