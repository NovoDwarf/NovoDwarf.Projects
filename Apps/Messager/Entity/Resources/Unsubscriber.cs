namespace Messager.Entity.Resources;

public class Unsubscriber : IDisposable
{
	private Action? _unsubscribe;

	public Unsubscriber(Action unsubscribe)
	{
		_unsubscribe = unsubscribe;
	}

	public void Dispose()
	{
		_unsubscribe?.Invoke();
		_unsubscribe = null;
	}
}