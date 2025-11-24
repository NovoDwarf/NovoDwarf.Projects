using Mathematics.Core.Base;
using Mathematics.Core.Enums.Cryptography;

namespace Mathematics.Sortings.Merge;

public class MergeSort<T> : Sorting<T>
	where T : IComparable<T>
{
	public MergeSort() : base(SpaceComplexityType.Linear, TimeComplexityType.Linearithmic, CryptoAlgorithmType.ComparisonSort)
	{ }

	public override void Sort(T[] array)
	{
		if (array is not { Length: > 1 })
			throw new ArgumentException("Array must have at least two elements");

		var temp = new T[array.Length];

		Sort(array, temp, 0, array.Length - 1);
	}

	private static void Sort(T[] array, T[] temp, int left, int right)
	{
		if (left >= right)
			return;

		var middle = (left + right) / 2;

		Sort(array, temp, left, middle);
		Sort(array, temp, middle + 1, right);
		Merge(array, temp, left, middle, right);
	}

	private static void Merge(T[] array, T[] temp, int left, int middle, int right)
	{
		var leftIndex = left;
		var rightIndex = middle + 1;
		var tempIndex = left;

		while (leftIndex <= middle && rightIndex <= right)
		{
			if (array[leftIndex].CompareTo(array[rightIndex]) <= 0)
			{
				temp[tempIndex] = array[leftIndex];
				leftIndex++;
			}
			else
			{
				temp[tempIndex] = array[rightIndex];
				rightIndex++;
			}

			tempIndex++;
		}

		while (leftIndex <= middle)
		{
			temp[tempIndex] = array[leftIndex];
			leftIndex++;
			tempIndex++;
		}

		while (rightIndex <= right)
		{
			temp[tempIndex] = array[rightIndex];
			rightIndex++;
			tempIndex++;
		}

		for (var i = left; i <= right; i++) array[i] = temp[i];
	}
}