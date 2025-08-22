using Godot;
using System;
using System.Collections.Generic;

public partial class CollisionCone : CollisionPolygon2D
{
	private float _radius;
	private float _angleRadians;
	private int _segments;
	public float Radius
	{
		get { return _radius; }
		set { _radius = value; }
	}
	public float AngleRadians
	{
		get { return _angleRadians; }
		set { _angleRadians = value; }
	}
	public int Segments
	{
		get { return _segments; }
		set { _segments = value; }
	}

	public override void _Ready()
	{
		float x = (float)(_radius * Math.Cos(_angleRadians));
		float y = (float)(_radius * Math.Sin(_angleRadians));
		List<Vector2> points = new()
		{
			Position,
		};

		float step = _angleRadians;

		if (_segments > 0)
		{
			step = _angleRadians / _segments;
		}

		float stepAngle = 0 - _angleRadians / 2;
		for (int i = 0; i < _segments + 1; i++)
		{
			x = (float)(_radius * Math.Cos(stepAngle));
			y = (float)(_radius * Math.Sin(stepAngle));
			points.Add(new Vector2(x, y));
			stepAngle += step;
		}

		Polygon = points.ToArray();
	}
}
