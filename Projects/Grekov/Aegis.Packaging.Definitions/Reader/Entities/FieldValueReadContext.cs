using System.Reflection;
using System.Xml.Linq;
using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Reader.Services;

namespace Aegis.Packaging.Definitions.Reader.Entities;

internal sealed record FieldValueReadContext(DefinitionXmlReader Reader, object OwnerInstance, FieldMetadata FieldMetadata, XElement OwnerElement,
	string PackageId,
	string ResourcePath,
	List<PendingReference> PendingReferences,
	FieldValueSource Source,
	XElement? ValueElement,
	string? AttributeValue)
{
	public DefFieldAttribute Field => FieldMetadata.Attribute;
	public PropertyInfo Property => FieldMetadata.Property;
	public Type PropertyType => FieldMetadata.PropertyType;
	public string FieldName => FieldMetadata.FieldName;
}