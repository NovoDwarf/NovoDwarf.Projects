using Grekov.Core;
using Grekov.Definitions.Entities;

namespace Grekov.Definitions.Interfaces;

public interface IDefFormatReader
{
	public IReadOnlySet<string> Extensions { get; }

	public void RefreshTypeMap();

	public IReadOnlyList<Def> ReadDefs(DefReadContext context);
}
