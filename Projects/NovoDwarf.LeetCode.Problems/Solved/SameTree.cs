using NovoDwarf.LeetCode.Problems.Models;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class SameTree
{
	public static bool Solve(TreeNode? p, TreeNode? q)
	{
		if (p == null || q == null)
			return p == null && q == null;
		
		if (p.Value != q.Value) 
			return false;
		
		var result = Solve(p.Left, q.Left) && Solve(p.Right, q.Right);

		return result;
	}
}