namespace Aegis.Packaging.Definitions.Reader.Entities;

internal sealed record TypeMetadata(Type Type, Func<object> Factory, FieldMetadata[] Fields);
