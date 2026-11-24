namespace NovoDwarf.LeetCode.Problems.Solved;

public static class PlusOne 
{
    public static int[] Solve(int[] digits) 
    {
        var i = digits.Length - 1;
        var carry = 1;

        while (i >= 0 && carry > 0)
        {
            var sum = digits[i] + carry;

            digits[i] = sum % 10;
            carry = sum / 10;

            i--;
        }

        if (carry <= 0)
            return digits;
        
        var result = new int[digits.Length + 1];
        result[0] = carry;
        
        return result;
    }
}