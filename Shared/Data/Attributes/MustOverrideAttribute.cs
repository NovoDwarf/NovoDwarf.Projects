namespace Data.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MustOverrideAttribute : Attribute
{
	public string Message { get; }
	public bool IsCritical { get; }

	public MustOverrideAttribute(string message = "Method must be overridden in derived class", bool isCritical = true)
	{
		Message = message;
		IsCritical = isCritical;
	}
}