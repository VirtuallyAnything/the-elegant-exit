using Godot;
using System;

public partial class Wall : StaticBody2D
{
	private CollisionShape2D _collisionShape2D;
	private LightOccluder2D _lightOccluder = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collisionShape2D = GetChild<CollisionShape2D>(0);
		RectangleShape2D wallRect = (RectangleShape2D)_collisionShape2D.Shape;
		_lightOccluder.Occluder = new OccluderPolygon2D()
		{
			Polygon = wallRect.Size.ToVertices(_collisionShape2D.Position, true)
		};
		_lightOccluder.AddToGroup("Occluder", true);
		AddChild(_lightOccluder);
	}
}
