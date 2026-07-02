using Aegis.Packaging.Definitions.Reader.Entities;
using Aegis.Packaging.Definitions.Reader.Interfaces;

namespace Aegis.Packaging.Definitions.Reader.Strategies;

internal sealed class XmlScalarValueStrategy : IXmlValueStrategy
{
	public bool CanHandle(XmlValueReadContext context)
	{
		var targetType = Nullable.GetUnderlyingType(context.TargetType) ?? context.TargetType;
		return !typeof(Resource).IsAssignableFrom(targetType)
		       && !typeof(Def).IsAssignableFrom(targetType)
		       && !typeof(ComponentDef).IsAssignableFrom(targetType);
	}

	public FieldReadResult Read(XmlValueReadContext context)
	{
		if (string.IsNullOrWhiteSpace(context.RawValue))
			return FieldReadResult.Missing();

		var targetType = Nullable.GetUnderlyingType(context.TargetType) ?? context.TargetType;
		var raw = context.RawValue.Trim();

		if (targetType == typeof(string))
			return FieldReadResult.Success(raw);

		if (targetType == typeof(int) && ScalarParser.TryParseInt32(raw, out var intValue))
			return FieldReadResult.Success(intValue);

		if (targetType == typeof(uint) &&
		    ScalarParser.TryParseUInt32(raw, out var uintValue))
			return FieldReadResult.Success(uintValue);

		if (targetType == typeof(long) &&
		    ScalarParser.TryParseInt64(raw, out var longValue))
			return FieldReadResult.Success(longValue);

		if (targetType == typeof(ulong) &&
		    ScalarParser.TryParseUInt64(raw, out var ulongValue))
			return FieldReadResult.Success(ulongValue);

		if (targetType == typeof(short) &&
		    ScalarParser.TryParseInt16(raw, out var shortValue))
			return FieldReadResult.Success(shortValue);

		if (targetType == typeof(byte) &&
		    ScalarParser.TryParseByte(raw, out var byteValue))
			return FieldReadResult.Success(byteValue);

		if (targetType == typeof(float) && ScalarParser.TryParseSingleInvariant(raw, out var floatValue))
			return FieldReadResult.Success(floatValue);

		if (targetType == typeof(double) && ScalarParser.TryParseDoubleInvariant(raw, out var doubleValue))
			return FieldReadResult.Success(doubleValue);

		if (targetType == typeof(decimal) && ScalarParser.TryParseDecimalInvariant(raw, out var decimalValue))
			return FieldReadResult.Success(decimalValue);

		if (targetType == typeof(bool) && ScalarParser.TryParseBool(raw, out var boolValue))
			return FieldReadResult.Success(boolValue);

		if (targetType == typeof(Color) && TryParseColor(raw, out var colorValue))
			return FieldReadResult.Success(colorValue);

		if (targetType == typeof(Vector2I))
		{
			var parts = raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2 &&
			    ScalarParser.TryParseInt32(parts[0], out var x) &&
			    ScalarParser.TryParseInt32(parts[1], out var y))
			{
				return FieldReadResult.Success(new Vector2I(x, y));
			}
		}

		if (targetType == typeof(NodePath))
			return FieldReadResult.Success(new NodePath(raw));

		if (targetType.IsEnum)
		{
			if (ScalarParser.TryParseEnum(targetType, raw, out var enumValue))
				return FieldReadResult.Success(enumValue);
		}

		return FieldReadResult.Invalid(
			$"Failed to parse value '{raw}' as '{context.TargetType.Name}' for member '{context.MemberName}' " +
			$"on type '{context.OwnerTypeName}' in '{context.ResourcePath}'.");
	}

	private static bool TryParseColor(string raw, out Color color)
	{
		var parts = raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length is 3 or 4)
		{
			if (ScalarParser.TryParseSingleInvariant(parts[0], out var r) &&
			    ScalarParser.TryParseSingleInvariant(parts[1], out var g) &&
			    ScalarParser.TryParseSingleInvariant(parts[2], out var b))
			{
				var a = 1f;
				if (parts.Length == 4 &&
				    !ScalarParser.TryParseSingleInvariant(parts[3], out a))
				{
					color = default;
					return false;
				}

				color = new Color(r, g, b, a);
				return true;
			}
		}

		if (Color.HtmlIsValid(raw))
		{
			color = Color.FromHtml(raw);
			return true;
		}

		color = default;
		return false;
	}
}
