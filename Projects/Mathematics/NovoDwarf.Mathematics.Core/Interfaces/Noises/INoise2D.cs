namespace Mathematics.Core.Interfaces.Noises;

public interface INoise2D
{
	public float Make(float x, float y);
}

public interface INoise4D
{
	public float Make(float x, float y, float z, float w);
}