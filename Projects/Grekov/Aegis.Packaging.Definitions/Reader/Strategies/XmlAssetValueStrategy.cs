using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;
using Aegis.Packaging.Definitions.Reader.Parsers;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class XmlAssetValueStrategy : IXmlValueStrategy
{
	public bool CanHandle(XmlValueReadContext context)
	{
		return typeof(Resource).IsAssignableFrom(context.TargetType);
	}

	public FieldReadResult Read(XmlValueReadContext context)
	{
		var path = context.ValueElement == null
			? context.RawValue
			: DefinitionXmlValueParsers.GetAttribute(context.ValueElement, "path") ?? context.RawValue;

		if (string.IsNullOrWhiteSpace(path))
		{
			return FieldReadResult.Invalid(
				$"Asset member '{context.MemberName}' in '{context.ResourcePath}' does not contain a resource path.");
		}

		var loaded = ResourceLoader.Load(path);
		if (loaded == null)
		{
			return FieldReadResult.Invalid(
				$"Failed to load resource '{path}' for member '{context.MemberName}' in '{context.ResourcePath}'.");
		}

		if (!context.TargetType.IsInstanceOfType(loaded))
		{
			return FieldReadResult.Invalid(
				$"Loaded resource '{path}' for member '{context.MemberName}' in '{context.ResourcePath}' has type " +
				$"'{loaded.GetType().FullName}', expected '{context.TargetType.FullName}'.");
		}

		return FieldReadResult.Success(loaded);
	}
}
