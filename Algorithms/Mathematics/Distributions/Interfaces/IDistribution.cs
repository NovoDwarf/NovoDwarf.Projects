namespace Mathematics.Distributions.Interfaces;

public interface IDistribution<out T>
{
	public T Calculate();
}