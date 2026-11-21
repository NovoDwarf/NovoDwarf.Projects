using NovoDwarf.LeetCode.Problems.Models;

namespace NovoDwarf.LeetCode.Problems;

public class ReverseLinkedList
{
	public ListNode Solve(ListNode head)
	{
		ListNode? prev = null;
		var curr = head;
		ListNode? next = null;

		// 0 -> 1 -> 2 -> null
		
		while (curr != null)
		{
			next = curr.Next; // next = null, curr.next = 1
			curr.Next = prev; // curr.next = null
			prev = curr; // prev = null, curr = 0, prev = 0
			curr = next; // curr = 0, next = 1, curr = 1
		}

		// curr == null, next == null, prev != null
		
		return prev;
	}
}