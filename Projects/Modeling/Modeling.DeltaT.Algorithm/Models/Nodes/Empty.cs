using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.DeltaT.Algorithm.Models.Nodes;

public sealed class Empty : EmptyBase
{
	public Empty(EmptyOptions? options = null) : base(options) { }
}