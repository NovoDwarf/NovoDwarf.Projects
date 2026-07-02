using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class XmlObjectValueStrategy : IXmlValueStrategy
{
	public bool CanHandle(XmlValueReadContext context)
	{
		return context.ValueElement != null &&
		       (typeof(ComponentDef).IsAssignableFrom(context.TargetType) ||
		        context.TargetType is { IsAbstract: false, IsInterface: false });
	}

	public FieldReadResult Read(XmlValueReadContext context)
	{
		try
		{
			var nested = context.Reader.ParseObject(
				context.TargetType,
				context.ValueElement!,
				context.PackageId,
				context.ResourcePath,
				null,
				context.PendingReferences);

			return FieldReadResult.Success(nested);
		}
		catch (Exception ex)
		{
			return FieldReadResult.Invalid(
				$"Failed to parse member '{context.MemberName}' as '{context.TargetType.Name}' " +
				$"on type '{context.OwnerTypeName}' in '{context.ResourcePath}': {ex.Message}");
		}
	}
}
