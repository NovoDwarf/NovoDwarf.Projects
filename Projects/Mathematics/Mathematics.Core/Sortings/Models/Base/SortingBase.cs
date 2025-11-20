using Mathematics.Core.Sortings.Enums;
using Mathematics.Core.Sortings.Interfaces;

namespace Mathematics.Core.Sortings.Models.Base;

public abstract class SortingBase<T> : ISorting<T>
	where T : IComparable<T>
{
	private readonly AlgorithmType _algorithmType;
	private readonly SpaceComplexityType _spaceComplexityType;
	private readonly TimeComplexityType _timeComplexityType;

	protected SortingBase(SpaceComplexityType spaceComplexityType, TimeComplexityType timeComplexityType,
		AlgorithmType algorithmType)
	{
		_spaceComplexityType = spaceComplexityType;
		_timeComplexityType = timeComplexityType;
		_algorithmType = algorithmType;
	}

	public string SpaceComplexity => GetSpaceComplexity();
	public string TimeComplexity => GetTimeComplexity();
	public string Algorithm => GetAlgorithmType();

	public abstract void Sort(T[] array);

	private string GetAlgorithmType()
	{
		return _algorithmType switch
		{
			AlgorithmType.ComparisonSort => "Comparison Sort",
			AlgorithmType.NonComparisonSort => "Non-Comparison Sort",
			AlgorithmType.Hybrid => "Hybrid",
			AlgorithmType.Parallel => "Parallel",
			AlgorithmType.External => "External",
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