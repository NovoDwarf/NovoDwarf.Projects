using System.Reflection;

namespace Messager.Core;

public class WeakAction<T>
{
	private readonly WeakReference? _targetRef;
	private readonly MethodInfo _method;
	private readonly bool _isStatic;

	public WeakAction(Action<T> action)
	{
		_method = action.Method;
		_isStatic = _method.IsStatic;

		_targetRef = _isStatic ? null : new WeakReference(action.Target!);
	}

	public bool IsAlive => _isStatic || (_targetRef?.IsAlive ?? false);

	public bool TryInvoke(T arg)
	{
		if (!_isStatic && !IsAlive)
			return false;

		var target = _isStatic ? null : _targetRef!.Target;
		if (!_isStatic && target == null)
			return false;

		_method.Invoke(target, [arg]);
		return true;
	}

	public bool Matches(Action<T> action)
	{
		if (action.Method != _method) 
			return false;
		
		return _isStatic || ReferenceEquals(action.Target, _targetRef!.Target);
	}
}