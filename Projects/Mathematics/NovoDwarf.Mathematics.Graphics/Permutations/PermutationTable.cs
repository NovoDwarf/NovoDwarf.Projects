namespace Mathematics.Graphics.Permutations;

public class PermutationTable
{
	private int[] _permutation = null!;
	private int[] _p = null!;
    
	public int Size { get; private set; }
	public int DoubleSize { get; private set; }

	public PermutationTable(int size = 256, long seed = 0) => Permutate(size, seed);

	public int this[int index] => _p[index & (DoubleSize - 1)];

	public int Hash(int x) => this[x];

	public int Hash(int x, int y) => this[this[x] + y];

	public int Hash(int x, int y, int z) => this[this[this[x] + y] + z];
    
	public int Hash(int x, int y, int z, int w) => this[this[this[this[x] + y] + z] + w];
    
	public PermutationTable WithSeed(long newSeed) => new(Size, newSeed);

	private void Permutate(int size, long seed)
	{
		Size = size;
		DoubleSize = size * 2;
        
		var random = new Random((int)seed);
		_permutation = new int[Size];
        
		for (var i = 0; i < Size; i++) 
			_permutation[i] = i;
        
		for (var i = 0; i < Size; i++)
		{
			var j = random.Next(i, Size);
            
			(_permutation[i], _permutation[j]) = (_permutation[j], _permutation[i]);
		}
        
		_p = new int[DoubleSize];
        
		for (var i = 0; i < DoubleSize; i++) 
			_p[i] = _permutation[i % Size];
	}
}