namespace NovoDwarf.LeetCode.Problems.Solved;

public static class FindGreatestCommonDivisorOfArray
{
    public static int Solve(int[] nums) 
    {
        var result = 0;

        var min = nums[0];
        var max = nums[0];

        foreach (var number in nums)
        {
            if (number > max)
                max = number;

            if (number < min)
                min = number;
        }

        result = Gcd(min, max);

        return result;
    }

    private static int Gcd(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);

        while (b != 0)
            (a, b) = (b, a % b);

        return a;
    }
}