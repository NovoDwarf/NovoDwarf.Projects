using System.Numerics;

namespace Mathematics.Ballistics.Entities.Precreated;

public class DragForce : IForce
{
	private readonly float _cd;
	private readonly float _area;

	public DragForce(float diameter, float cd)
	{
		_cd = cd;
		_area = (float)(Math.PI * Math.Pow(diameter / 2.0, 2));
	}

	public Vector3 Compute(Projectile p, World w)
	{
		var v = p.Velocity;
		var speed = v.Length();
		
		if (speed == 0) 
			return new Vector3();

		var dragMag = 0.5f * w.AirDensity * speed * speed * _cd * _area;
		
		return v.Normalized() * -dragMag;
	}
}