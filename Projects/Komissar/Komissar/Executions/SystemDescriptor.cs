namespace Komissar.Executions;

public sealed class SystemDescriptor
{
    public static SystemDescriptor Exclusive { get; } = new();

    public SystemDescriptor(
        IEnumerable<Type>? reads = null,
        IEnumerable<Type>? writes = null)
    {
        Reads = (reads ?? []).ToHashSet();
        Writes = (writes ?? []).ToHashSet();
    }

    public IReadOnlySet<Type> Reads { get; }
    public IReadOnlySet<Type> Writes { get; }

    public bool CanRunInParallelWith(SystemDescriptor other)
    {
        return !Writes.Overlaps(other.Reads)
            && !Writes.Overlaps(other.Writes)
            && !other.Writes.Overlaps(Reads);
    }
}
