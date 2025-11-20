namespace ShaderEditor.Interfaces;

public interface IPanel
{
	public string Panel { get; }

	public void Render();
}

public abstract class BasePanel : IPanel
{
	public abstract string Panel { get; }

	public abstract void Render();
}