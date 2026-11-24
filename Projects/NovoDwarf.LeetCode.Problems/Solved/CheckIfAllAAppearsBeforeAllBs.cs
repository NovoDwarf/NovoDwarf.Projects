namespace NovoDwarf.LeetCode.Problems.Solved;

public static class CheckIfAllAAppearsBeforeAllBs
{
	public static bool Solve(string s) {
        
		var foundB = false;

		foreach (var t in s)
		{
			if (t == 'b')
				foundB = true;
            
			if (foundB && t == 'a')
				return false;
		}

		return true;
	}
}