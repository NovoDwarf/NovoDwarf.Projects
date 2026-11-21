namespace NovoDwarf.LeetCode.Problems.Models;

public class TreeNode
{
	public TreeNode(int value = 0, TreeNode? left = null, TreeNode? right = null)
	{
		Value = value;
		Left = left;
		Right = right;
	}

	public int Value { get; init; }
	public TreeNode? Left { get; init; }
	public TreeNode? Right { get; init; }
	
}