using Grekov.Core;
using Grekov.Definitions.Entities;

namespace Grekov.Definitions.Interfaces;

public interface IDefinitionFormatReader
{
	IReadOnlySet<string> Extensions { get; }

	void RefreshTypeMap();

	IReadOnlyList<Def> ReadDefinitions(DefinitionReadContext context);
}
