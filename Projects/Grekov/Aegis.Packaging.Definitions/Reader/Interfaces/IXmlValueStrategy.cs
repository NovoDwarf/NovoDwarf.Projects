using Aegis.Packaging.Definitions.Reader.Entities;

namespace Aegis.Packaging.Definitions.Reader.Interfaces;

internal interface IXmlValueStrategy
{
	public bool CanHandle(XmlValueReadContext context);
	public FieldReadResult Read(XmlValueReadContext context);
}
