namespace NovoDwarf.LeetCode.Problems.Solved;

public class FibonacciNumber
{
	private readonly Dictionary<int, int> _cache = new();
	
	public int Solve(int n)
	{
		return Fibbonaci(n);
	}

	private int Fibbonaci(int n)
	{
		if (n <= 1)
			return n;

		if (_cache.TryGetValue(n, out var fibbonaci))
			return fibbonaci;
		
		_cache[n] = Fibbonaci(n - 1) + Fibbonaci(n - 2);

		return _cache[n];
	}
}