using System.Numerics;

namespace Mathematics.Ballistics;

public static class VectorExtensions
{
	extension(Vector3 vector)
	{
		public Vector3 Normalized()
		{
			var len = vector.Length();
			return len != 0 
				? new Vector3(vector.X / len, vector.Y / len, vector.Z / len) 
				: new Vector3();
		}
	}
}