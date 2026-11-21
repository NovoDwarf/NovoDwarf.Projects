namespace NovoDwarf.LeetCode.Problems.Models;

public class ListNode
{
	public ListNode(int value = 0, ListNode? next = null)
	{
		Value = value;
		Next = next;
	}

	public int Value { get; set; }
	public ListNode? Next { get; set; }
	
}