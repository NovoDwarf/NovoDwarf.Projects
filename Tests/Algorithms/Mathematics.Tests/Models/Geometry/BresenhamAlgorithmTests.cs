using Mathematics.Geometry.Models;

namespace Mathematics.Tests.Models.Geometry;

[TestFixture]
public class BresenhamAlgorithmTests
{
	[Test]
	public void DrawLine_PointsAreConnected_NoGaps()
	{
		var points = BresenhamAlgorithm.DrawLine(0, 0, 10, 3);

		for (var i = 1; i < points.Count; i++)
		{
			var dx = Math.Abs(points[i].X - points[i - 1].X);
			var dy = Math.Abs(points[i].Y - points[i - 1].Y);
			
			TestContext.Out.WriteLine($"Point {points[i-1]} -> {points[i]}: dX: {dx}; dY: {dy}");
			
			Assert.That(dx <= 1 && dy <= 1, $"Points {points[i-1]} and {points[i]} are not adjacent");
		}
	}
}