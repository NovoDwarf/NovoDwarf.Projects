namespace NovoDwarf.LeetCode.Problems.Solved;

public class PrintFooBarAlternaly
{
	private readonly int _n;

	private readonly SemaphoreSlim _fooSemaphore = new(1, 1);
	private readonly SemaphoreSlim _barSemaphore = new(0, 1);

	public PrintFooBarAlternaly(int n)
	{
		_n = n;
	}

	public void Foo(Action printFoo)
	{
		for (var i = 0; i < _n; i++)
		{
			_fooSemaphore.Wait();
			printFoo();
			_barSemaphore.Release();
		}
	}

	public void Bar(Action printBar)
	{
		for (var i = 0; i < _n; i++)
		{
			_barSemaphore.Wait();
			printBar();
			_fooSemaphore.Release();
		}
	}
}