using Structures.Models.Abstracts.Nodes;
using Structures.Models.Abstracts.Options;

namespace DeltaT.Algorithm.Models.Nodes;

public sealed class Empty : EmptyBase
{
	public Empty(EmptyOptions? options = null) : base(options) { }
}