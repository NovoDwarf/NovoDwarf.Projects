namespace Sorting.Interfaces;

public interface ISorting<in T> where T : IComparable<T>
{
	public void Sort(T[] array);
}