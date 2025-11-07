namespace Sorting.Tests.Merge;

public class Person : IComparable<Person>
{
	public string Name { get; }
	public int Age { get; }

	public Person(string name, int age)
	{
		Name = name;
		Age = age;
	}

	public int CompareTo(Person? other)
	{
		return other != null 
			? Age.CompareTo(other.Age) 
			: throw new NullReferenceException();
	}
}