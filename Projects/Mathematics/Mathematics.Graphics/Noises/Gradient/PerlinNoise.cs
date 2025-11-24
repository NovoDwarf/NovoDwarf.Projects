using Mathematics.Core.Utilities;
using Mathematics.Graphics.Permutations;

namespace Mathematics.Graphics.Noises.Gradient;

public partial class PerlinNoise
{
    private readonly PermutationTable _permutation;

    public int Seed { get; }
    public int Size { get; }

    public PerlinNoise(int size = 256, int? seed = null)
    {
        Seed = seed ?? RandomUtils.Next();
        Size = size;
        
        _permutation = new PermutationTable(Size, Seed);
    }
    
    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    
    private static float Lerp(float t, float a, float b) => a + t * (b - a);
}