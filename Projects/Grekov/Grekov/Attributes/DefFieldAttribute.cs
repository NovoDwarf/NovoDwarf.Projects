using Grekov.Enums;

namespace Grekov.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DefFieldAttribute : Attribute
{
	public DefFieldAttribute(string? name = null)
	{
		Name = name;
	}

	public string? Name { get; }
	public DefFieldKind Kind { get; init; } = DefFieldKind.Auto;
	public bool Required { get; init; }
	public string? ItemName { get; init; }
}