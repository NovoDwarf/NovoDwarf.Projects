namespace Mathematics.Core.Distributions.Interfaces;

public interface IDistribution<out T>
{
	public T Distribute();
}