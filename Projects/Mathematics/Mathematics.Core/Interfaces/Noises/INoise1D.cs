namespace Mathematics.Core.Interfaces.Noises;

public interface INoise1D<T>
{
	public T Make(T x);
}