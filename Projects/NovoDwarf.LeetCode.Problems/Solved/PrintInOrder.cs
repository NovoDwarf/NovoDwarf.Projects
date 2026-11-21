namespace NovoDwarf.LeetCode.Problems.Solved;

public class PrintInOrder
{
	private readonly ManualResetEventSlim _firstDone = new(false);
	private readonly ManualResetEventSlim _secondDone = new(false);

	public void First(Action printFirst)
	{
		printFirst();
		_firstDone.Set();
	}

	public void Second(Action printSecond)
	{
		_firstDone.Wait();
		printSecond();
		_secondDone.Set();
	}

	public void Third(Action printThird)
	{
		_secondDone.Wait();
		printThird();
	}
}