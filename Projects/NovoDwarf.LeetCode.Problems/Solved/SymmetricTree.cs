using NovoDwarf.LeetCode.Problems.Models;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class SymmetricTree
{
	public static bool Solve(TreeNode root)
	{
		return Solve(root.Left, root.Right);
	}

	private static bool Solve(TreeNode? p, TreeNode? q)
	{
		if (p == null || q == null)
			return p == null && q == null;
		
		if (p.Value != q.Value) 
			return false;
		
		var result = Solve(p.Left, q.Right) && Solve(p.Right, q.Left);

		return result;
	}
}