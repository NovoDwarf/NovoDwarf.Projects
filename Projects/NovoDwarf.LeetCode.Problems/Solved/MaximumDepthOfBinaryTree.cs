using NovoDwarf.LeetCode.Problems.Models;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class MaximumDepthOfBinaryTree
{
	public static int Solve(TreeNode? root)
	{
		if (root == null)
			return 0;

		var leftDepth = Solve(root.Left);
		var rightDepth = Solve(root.Right);

		return Math.Max(leftDepth, rightDepth) + 1;	
	}
}