using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;
using Aegis.Packaging.Definitions.Reader.Parsers;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class XmlReferenceValueStrategy : IXmlValueStrategy
{
	public bool CanHandle(XmlValueReadContext context)
	{
		return typeof(Def).IsAssignableFrom(context.TargetType) && context.ValueElement != null;
	}

	public FieldReadResult Read(XmlValueReadContext context)
	{
		var referenceIdRaw = DefinitionXmlValueParsers.GetAttribute(context.ValueElement!, "ref");
		if (!string.IsNullOrWhiteSpace(referenceIdRaw))
		{
			DefId parsedReferenceId;

			try
			{
				parsedReferenceId = DefId.Parse(referenceIdRaw);
			}
			catch (Exception ex)
			{
				return FieldReadResult.Invalid(
					$"Invalid reference id '{referenceIdRaw}' for member '{context.MemberName}' " +
					$"on type '{context.OwnerTypeName}' in '{context.ResourcePath}': {ex.Message}");
			}

			if (context.DeferredValueSetter == null)
			{
				return FieldReadResult.Invalid(
					$"Reference member '{context.MemberName}' on type '{context.OwnerTypeName}' in '{context.ResourcePath}' " +
					$"does not provide a deferred setter.");
			}

			context.PendingReferences.Add(new PendingReference(
				context.TargetType,
				parsedReferenceId.ToString(),
				context.ResourcePath,
				context.DeferredValueSetter));

			return FieldReadResult.Success(PendingValue.Instance);
		}

		var nested = context.Reader.ParseObject(
			context.TargetType,
			context.ValueElement!,
			context.PackageId,
			context.ResourcePath,
			null,
			context.PendingReferences);

		return FieldReadResult.Success(nested);
	}
}
