using System.Reflection;

namespace Messager.Core;

public class WeakAction<T>
{
	private readonly WeakReference _targetRef;
	private readonly MethodInfo _method;

	public WeakAction(Action<T> action)
	{
		_targetRef = new WeakReference(action.Target);
		_method = action.Method;
	}

	public bool IsAlive => _targetRef.IsAlive;

	public bool TryInvoke(T arg)
	{
		if (!IsAlive) 
			return false;

		var target = _targetRef.Target;
		
		if (target == null) 
			return false;

		_method.Invoke(target, [arg]);
		
		return true;
	}

	public bool Matches(Action<T> action)
	{
		return action.Method == _method && action.Target == _targetRef.Target;
	}
}