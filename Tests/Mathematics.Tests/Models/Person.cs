namespace Mathematics.Tests.Models;

public class Person : IComparable<Person>
{
	public Person(string name, int age)
	{
		Name = name;
		Age = age;
	}

	public string Name { get; }
	public int Age { get; }

	public int CompareTo(Person? other)
	{
		return other != null
			? Age.CompareTo(other.Age)
			: throw new NullReferenceException();
	}
}