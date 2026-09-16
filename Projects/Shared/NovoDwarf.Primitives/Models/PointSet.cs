using System;
using System.Collections;
using System.Collections.Generic;

namespace NovoDwarf.Primitives.Models;

public class PointSet
{
	public PointSet(int width, int height, SpatialPoint[] points)
	{
		Points = points;
	}

	public SpatialPoint[] Points { get; set; }

	public static PointSet Empty(int width, int height)
	{
		throw new NotImplementedException();
	}
}

public record struct SpatialPoint
{
	public SpatialPoint(Float2.Float2 position, float radius, float weight)
	{
		Position = position;
		Radius = radius;
		Weight = weight;
	}
	
	public Float2.Float2 Position { get; set; }
	public float Radius { get; set; }
	public float Weight { get; set; }
	public float X { get; set; }
	public float Y { get; set; }
}

public record struct SpatialPath
{
	public SpatialPath(List<Float2.Float2> position, float radius, float weight)
	{
		Radius = radius;
		Weight = weight;
	}
	
	public SpatialPath(Float2.Float2 position, float radius, float weight)
	{
		Position = position;
		Radius = radius;
		Weight = weight;
	}
	
	public Float2.Float2 Position { get; set; }
	public float Radius { get; set; }
	public float Weight { get; set; }
	public Float2.Float2[] Points { get; set; }
	
	public double Width { get; set; }
}

public class PathSet
{
	public PathSet(int width, int height, List<SpatialPath> paths)
	{
		throw new NotImplementedException();
	}

	public SpatialPath[] Paths { get; set; }

	public static PathSet Empty(int width, int height)
	{
		throw new NotImplementedException();
	}
}