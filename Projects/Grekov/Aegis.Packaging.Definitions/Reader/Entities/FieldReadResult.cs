namespace Aegis.Packaging.Definitions.Reader.Entities;

internal sealed record FieldReadResult(
	FieldReadStatus Status,
	object? Value,
	string? ErrorMessage)
{
	public static FieldReadResult Success(object? value) => new(FieldReadStatus.Success, value, null);
	public static FieldReadResult Missing() => new(FieldReadStatus.Missing, null, null);
	public static FieldReadResult Invalid(string error) => new(FieldReadStatus.Invalid, null, error);
}