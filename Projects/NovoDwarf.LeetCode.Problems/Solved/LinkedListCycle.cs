using NovoDwarf.LeetCode.Problems.Models;

namespace NovoDwarf.LeetCode.Problems.Solved;

public static class LinkedListCycle
{
	public static bool Solve(ListNode head)
	{
		var slow = head;
		var fast = head;
		
		while (fast != null)
		{
			slow = slow?.Next;
			fast = fast.Next?.Next;

			if (slow != null && slow == fast)
				return true;
		}

		return false;
	}
}