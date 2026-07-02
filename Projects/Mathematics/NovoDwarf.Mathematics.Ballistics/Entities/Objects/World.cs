using System.Numerics;
using Mathematics.Ballistics.Entities.Data;

namespace Mathematics.Ballistics.Entities;

public class World
{
	public World(WorldData data)
	{
		Gravity = data.Gravity;
	}
	
	public Vector3 Gravity { get; set; }
	public float AirDensity { get; set; }
}