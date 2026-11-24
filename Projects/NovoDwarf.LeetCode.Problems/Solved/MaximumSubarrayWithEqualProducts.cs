namespace NovoDwarf.LeetCode.Problems.Solved;

public class MaximumSubarrayWithEqualProducts 
{
    public static int Solve(int[] nums) 
    {
        var max = 0;

        for (var i = 0; i < nums.Length; i++) 
        for (var j = i; j < nums.Length; j++) 
        {
            var subarray = nums.AsSpan(i, j - i + 1);

            if (Product(subarray) != Gcd(subarray) * Lcm(subarray)) 
                continue;
            
            var length = j - i + 1;
            max = Math.Max(max, length);
        }
        
        return max;
    }

    private static long Product(ReadOnlySpan<int> numbers)
    {
        long result = 1;

        foreach (var number in numbers)
            result *= number;

        return result;
    }

    private static long Gcd(ReadOnlySpan<int> numbers)
    {
        long result = 0;

        foreach (var number in numbers)
        {
            result = Gcd(result, number);

            if (result == 1)
                return 1;
        }

        return result;
    }

    private static long Lcm(ReadOnlySpan<int> numbers)
    {
        long result = 1;

        foreach (var number in numbers)
        {
            result = Lcm(result, number);

            if (result == 0)
                return 0;
        }

        return result;
    }

    private static long Gcd(long a, long b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);

        while (b != 0)
            (a, b) = (b, a % b);

        return a;
    }

    private static long Lcm(long a, long b)
    {
        if (a == 0 || b == 0)
            return 0;

        return checked(Math.Abs(a / Gcd(a, b) * b));
    }
}