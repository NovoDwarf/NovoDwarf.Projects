namespace Mathematics.Core.Interfaces;

public interface INoise1D<T>
{
	public T Make(T x);
}

public interface INoise2D<T>
{
	public T Make(T x, T y);
}

public interface INoise3D<T>
{
	public T Make(T x, T y, T z);
}