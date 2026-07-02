using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;
using Aegis.Packaging.Definitions.Reader.Parsers;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class ScalarFieldValueStrategy : IFieldValueStrategy
{
	public bool CanHandle(FieldValueReadContext context)
	{
		return context.Source switch
		{
			FieldValueSource.Attribute => context.Field.Kind is DefFieldKind.Auto or DefFieldKind.Attribute,
			FieldValueSource.Element => context.Field.Kind is not DefFieldKind.Collection,
			_ => false
		};
	}

	public FieldReadResult Read(FieldValueReadContext context)
	{
		var raw = context.Source == FieldValueSource.Attribute
			? context.AttributeValue
			: context.ValueElement!.HasElements
				? null
				: DefinitionXmlValueParsers.GetValue(context.ValueElement);

		return context.Reader.ReadValue(new XmlValueReadContext(
			context.Reader,
			context.PropertyType,
			context.Property.Name,
			context.OwnerInstance.GetType().Name,
			context.ValueElement,
			raw,
			context.PackageId,
			context.ResourcePath,
			context.PendingReferences,
			resolved => context.FieldMetadata.Setter(context.OwnerInstance, resolved)));
	}
}
