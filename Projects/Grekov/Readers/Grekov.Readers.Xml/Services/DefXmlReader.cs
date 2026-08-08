using Grekov.Core;
using Grekov.Definitions.Entities;
using Grekov.Definitions.Interfaces;

namespace Grekov.Readers.Xml.Services;

public sealed class DefXmlReader : IDefFormatReader
{
	private static readonly IReadOnlySet<string> SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		".xml"
	};

	public IReadOnlySet<string> Extensions => SupportedExtensions;

	public void RefreshTypeMap()
	{
	}

	public IReadOnlyList<Def> ReadDefs(DefReadContext context)
	{
		throw new NotSupportedException("XML definition reader is registered, but XML parsing is not implemented yet.");
	}
}
