namespace Mathematics.Core.Base.Graphics;

[Flags]
public enum OutCode
{ 
	Inside = 0,
	Left = 1,
	Right = 2,
	Bottom = 4,
	Top = 8
}