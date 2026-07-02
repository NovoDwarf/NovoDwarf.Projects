using System.Reflection;

namespace Aegis.Packaging.Definitions.Reader.Entities;

internal sealed record FieldMetadata(
	PropertyInfo Property,
	Type PropertyType,
	DefFieldAttribute Attribute,
	string FieldName,
	Action<object, object?> Setter);
