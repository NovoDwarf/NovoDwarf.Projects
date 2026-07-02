namespace NovoDwarf.Mathematics.App.Systems.Measurements.Domain;

public interface IDimension
{
	public IReadOnlyDictionary<string, int> Exponents { get; init; }

	public Dimension Multiply(Dimension other);

	public Dimension Divide(Dimension other);

	public Dimension Invert();

	public bool Equals(Dimension? other);
}

public sealed class Dimension : IDimension, IEquatable<Dimension>
{
	public Dimension(IDictionary<string, int> exponents)
	{
		Exponents = exponents
			.Where(e => e.Value != 0)
			.ToDictionary(e => e.Key, e => e.Value);
	}

	public IReadOnlyDictionary<string, int> Exponents { get; init; }

	public Dimension Multiply(Dimension other)
	{
		return new Dimension(Exponents
			.Concat(other.Exponents)
			.GroupBy(e => e.Key)
			.ToDictionary(
				g => g.Key,
				g => g.Sum(x => x.Value)));
	}

	public Dimension Divide(Dimension other)
	{
		return Multiply(other.Invert());
	}

	public Dimension Invert()
	{
		return new Dimension(Exponents.ToDictionary(e => e.Key, e => -e.Value));
	}

	public bool Equals(Dimension? other)
	{
		if (ReferenceEquals(this, other))
			return true;

		if (other is null)
			return false;

		if (Exponents.Count != other.Exponents.Count)
			return false;

		foreach (var (key, value) in Exponents)
		{
			if (!other.Exponents.TryGetValue(key, out var otherValue))
				return false;

			if (!EqualityComparer<int>.Default.Equals(value, otherValue))
				return false;
		}

		return true;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as Dimension);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hash = 17;

			foreach (var kv in Exponents.OrderBy(e => e.Key))
			{
				hash = hash * 23 + kv.Key.GetHashCode();
				hash = hash * 23 + kv.Value.GetHashCode();
			}

			return hash;
		}
	}
}