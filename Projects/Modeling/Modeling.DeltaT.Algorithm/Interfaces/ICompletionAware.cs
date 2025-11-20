using Modeling.Core.EX;
using Modeling.Core.Models.Base;

namespace Modeling.DeltaT.Algorithm.Interfaces;

public interface ICompletionAware
{
	void OnRequestCompleted(Request request, Simulation sim);
}