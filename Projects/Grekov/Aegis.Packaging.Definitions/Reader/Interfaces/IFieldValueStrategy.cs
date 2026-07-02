using Aegis.Packaging.Definitions.Reader.Entities;

namespace Aegis.Packaging.Definitions.Reader.Interfaces;

internal interface IFieldValueStrategy
{
	public bool CanHandle(FieldValueReadContext context);
	public FieldReadResult Read(FieldValueReadContext context);
}