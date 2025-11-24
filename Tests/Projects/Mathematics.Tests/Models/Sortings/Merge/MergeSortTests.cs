using Mathematics.Sortings.Merge;

namespace Mathematics.Tests.Models.Sortings.Merge;

[TestFixture]
public class MergeSortTests
{
	private readonly MergeSort<int> _intSorter = new();
	private readonly MergeSort<string> _stringSorter = new();
	private readonly MergeSort<double> _doubleSorter = new();
	private readonly MergeSort<Person> _personSorter = new();

	[Test]
	public void Sort_EmptyArray_ReturnsEmptyArray()
	{
		var array = Array.Empty<int>();

		Assert.Throws<ArgumentException>(() => _intSorter.Sort(array));
	}

	[Test]
	public void Sort_NullArray_ThrowsNoException()
	{
		int[] array = null;

		Assert.Throws<ArgumentException>(() => _intSorter.Sort(array));
	}

	[Test]
	[TestCase(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 })]
	public void Sort_AlreadySortedArray_RemainsSorted(int[] array, int[] expected)
	{
		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	[TestCase(new[] { 5, 4, 3, 2, 1 }, new[] { 1, 2, 3, 4, 5 })]
	public void Sort_ReverseSortedArray_BecomesSorted(int[] array, int[] expected)
	{
		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_UnsortedArray_BecomesSorted()
	{
		int[] array = [64, 34, 25, 12, 22, 11, 90];
		int[] expected = [11, 12, 22, 25, 34, 64, 90];

		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_ArrayWithDuplicates_BecomesSorted()
	{
		int[] array = [5, 2, 8, 2, 5, 1, 8];
		int[] expected = [1, 2, 2, 5, 5, 8, 8];

		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_ArrayWithNegativeNumbers_BecomesSorted()
	{
		int[] array = [-3, -1, -7, 0, 5, -2];
		int[] expected = [-7, -3, -2, -1, 0, 5];

		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_LargeArray_BecomesSorted()
	{
		var array = new int[1000];
		var random = new Random();
		for (var i = 0; i < array.Length; i++) array[i] = random.Next(-1000, 1000);

		_intSorter.Sort(array);

		Assert.That(IsSorted(array));
	}

	[Test]
	public void Sort_ArrayWithAllSameElements_RemainsSame()
	{
		int[] array = [7, 7, 7, 7, 7];
		int[] expected = [7, 7, 7, 7, 7];

		_intSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	private bool IsSorted(int[] array)
	{
		for (var i = 1; i < array.Length; i++)
			if (array[i] < array[i - 1])
				return false;
		return true;
	}

	[Test]
	public void Sort_StringArray_BecomesSorted()
	{
		string[] array = ["banana", "apple", "cherry", "date"];
		string[] expected = ["apple", "banana", "cherry", "date"];


		_stringSorter.Sort(array);


		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_DoubleArray_BecomesSorted()
	{
		double[] array = [3.14, 1.41, 2.71, 0.57];
		double[] expected = [0.57, 1.41, 2.71, 3.14];


		_doubleSorter.Sort(array);

		Assert.That(expected, Is.EqualTo(array));
	}

	[Test]
	public void Sort_CustomObjects_BecomesSortedByProperty()
	{
		var people = new[]
		{
			new Person("John", 25),
			new Person("Alice", 30),
			new Person("Bob", 20)
		};

		var expected = new[]
		{
			new Person("Bob", 20),
			new Person("John", 25),
			new Person("Alice", 30)
		};

		_personSorter.Sort(people);

		Assert.That(expected.Select(p => p.Name), Is.EqualTo(people.Select(p => p.Name)));
	}
}