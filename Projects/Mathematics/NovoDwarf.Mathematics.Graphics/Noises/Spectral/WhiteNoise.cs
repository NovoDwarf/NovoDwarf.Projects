using Mathematics.Core.Utilities;

namespace Mathematics.Graphics.Noises.Spectral;

public class WhiteNoise
{
	public int Seed { get; set; } = RandomUtils.Next();
	
	public float Make(float x)
	{
		unchecked
		{
			var h = (uint)(x * 374761393 + Seed * 668265263);
			h = (h ^ (h >> 13)) * 1274126177u;
			return (h & 0xFFFFFF) / (float)0xFFFFFF * 2f - 1f;
		}
	}

	public float Make(float x, float y)
	{
		unchecked
		{
			var h = (uint)(x * 374761393 + y * 668265263 + Seed * 1442695040888963407);
			h = (h ^ (h >> 13)) * 1274126177u;
			return (h & 0xFFFFFF) / (float)0xFFFFFF * 2f - 1f;
		}
	}
	
	public float Make(float x, float y, float z)
	{
		unchecked
		{
			var h = (uint)(x * 374761393 + y * 668265263 + z * 1442695041 + Seed * 2147483647);
			h = (h ^ (h >> 13)) * 1274126177u;
			return (h & 0xFFFFFF) / (float)0xFFFFFF * 2f - 1f;
		}
	}
}