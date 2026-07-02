namespace Mathematics.Core.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EntityParameterAttribute : Attribute
{
	public EntityParameterAttribute(Type type, string name) 
	{
		Type = type;
		NameKey = $"Param_{name}_Name";
		DescKey = $"Param_{name}_Desc";
	}

	public Type Type { get; private set; }
	
	public string NameKey { get; private set; }
	
	public string DescKey { get; private set; }
}