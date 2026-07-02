using System.Numerics;

namespace Mathematics.Ballistics.Entities;

public interface IForce
{
	public Vector3 Compute(Projectile p, World w);
}
