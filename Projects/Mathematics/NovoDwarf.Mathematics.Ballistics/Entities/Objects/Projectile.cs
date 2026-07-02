using System.Numerics;

namespace Mathematics.Ballistics.Entities;

public class Projectile
{
	public float Mass { get; }
	public Vector3 Position { get; set; }
	public Vector3 Velocity { get; set; }

	private readonly List<IForce> _forces = [];

	public Projectile(float mass)
	{
		Mass = mass;
	}

	public void AddForce(IForce force)
		=> _forces.Add(force);

	public void Step(double dt, World w)
	{
		Vector3 totalForce = new();

		foreach (var f in _forces)
			totalForce += f.Compute(this, w);

		var acceleration = totalForce * (1.0f / Mass);

		//Velocity += acceleration * dt;
		//Position += Velocity * dt;
	}
}