namespace Structures.Models.Base;

public record Range(double Min, double Max, bool IncludeMin = true, bool IncludeMax = false)
{
	public bool Contains(double value)
	{
		var minCheck = IncludeMin ? value >= Min : value > Min;
		var maxCheck = IncludeMax ? value <= Max : value < Max;
		
		return minCheck && maxCheck;
	}

	public override string ToString()
	{
		var leftBracket = IncludeMin ? "[" : "(";
		var rightBracket = IncludeMax ? "]" : ")";
		
		return $"{leftBracket}{Min}, {Max}{rightBracket}";
	}
}