using Structures.EX;
using Structures.Models.Base;

namespace DeltaT.Algorithm.Interfaces;

public interface ICompletionAware
{
	void OnRequestCompleted(Request request, Simulation sim);
}