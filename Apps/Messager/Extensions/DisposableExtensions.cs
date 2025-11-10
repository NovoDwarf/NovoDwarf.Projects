using Messager.Entity.Resources;

namespace Messager.Extensions;

public static class DisposableExtensions
{
	public static IDisposable AddTo(this IDisposable disposable, DisposableList list)
	{
		list.Add(disposable);
		return disposable;
	}
}