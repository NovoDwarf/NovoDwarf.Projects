using Mathematics.Core.Enums.Cryptography;

namespace Mathematics.Core.Base;

public abstract class Sorting<T> : Entity
	where T : IComparable<T>
{
	private readonly CryptoAlgorithmType _cryptoAlgorithmType;
	private readonly SpaceComplexityType _spaceComplexityType;
	private readonly TimeComplexityType _timeComplexityType;

	protected Sorting(SpaceComplexityType spaceComplexityType, TimeComplexityType timeComplexityType,
		CryptoAlgorithmType cryptoAlgorithmType)
	{
		_spaceComplexityType = spaceComplexityType;
		_timeComplexityType = timeComplexityType;
		_cryptoAlgorithmType = cryptoAlgorithmType;
	}
	
	public override string Category => "sorting_category";
	
	public string SpaceComplexity => GetSpaceComplexity();
	public string TimeComplexity => GetTimeComplexity();
	public string Algorithm => GetAlgorithmType();

	public abstract void Sort(T[] array);

	private string GetAlgorithmType()
	{
		return _cryptoAlgorithmType switch
		{
			CryptoAlgorithmType.ComparisonSort => "Comparison Sort",
			CryptoAlgorithmType.NonComparisonSort => "Non-Comparison Sort",
			CryptoAlgorithmType.Hybrid => "Hybrid",
			CryptoAlgorithmType.Parallel => "Parallel",
			CryptoAlgorithmType.External => "External",
			_ => "Unknown"
		};
	}

	private string GetSpaceComplexity()
	{
		return _spaceComplexityType switch
		{
			SpaceComplexityType.Constant => "O(1)",
			SpaceComplexityType.Logarithmic => "O(log n)",
			SpaceComplexityType.Linear => "O(n)",
			SpaceComplexityType.Quadratic => "O(n²)",
			_ => "Unknown"
		};
	}

	private string GetTimeComplexity()
	{
		return _timeComplexityType switch
		{
			TimeComplexityType.Constant => "O(1)",
			TimeComplexityType.Logarithmic => "O(log n)",
			TimeComplexityType.Linear => "O(n)",
			TimeComplexityType.Linearithmic => "O(n log n)",
			TimeComplexityType.Quadratic => "O(n²)",
			TimeComplexityType.Cubic => "O(n³)",
			TimeComplexityType.Exponential => "O(2ⁿ)",
			_ => "Unknown"
		};
	}
}