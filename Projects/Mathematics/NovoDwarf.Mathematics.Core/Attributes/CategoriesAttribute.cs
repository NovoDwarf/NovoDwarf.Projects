namespace Mathematics.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CategoriesAttribute : Attribute
{
	public IReadOnlyList<string> Path { get; }

	public CategoriesAttribute(params string[] paths)
	{
		Path = paths
			.Where(p => !string.IsNullOrEmpty(p))
			.Select(p => $"Category_{p}_Name")
			.ToArray();
	}
}