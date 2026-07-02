using System.Numerics;

namespace Mathematics.Ballistics.Entities.Precreated;

public class GravityForce
{
	public Vector3 Compute(Projectile p, World w) => w.Gravity * p.Mass;
}