namespace NovoDwarf.LeetCode.Problems.Solved;

public class ProductOfNumbers
{
    private readonly List<int> _multi = [1];

    public void Add(int num)
    {
        if (num == 0)
        {
            _multi.Clear();
            _multi.Add(1);
            return;
        }

        _multi.Add(_multi[^1] * num);
    }

    public int GetProduct(int k)
    {
        if (k >= _multi.Count)
            return 0;

        return _multi[^1] / _multi[^(k + 1)];
    }
}