using Godot;
using System;

namespace tee
{
	public abstract partial class PartyCharacter : Node2D
	{
		[Export] private int _sightConeSegments;
		[Export] private float _sightConeAngleDegrees;
		[Export] private float _sightConeRadius;
		[Export] protected float _movementSpeed;
		protected Movement _movement;
		protected Vision _vision;

		public Movement Movement
		{
			get { return _movement; }
		}

		public override void _Ready()
		{
			CollisionCone collisionCone = new()
			{
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};

			_vision.AddChild(collisionCone);
			AddChild(_vision);
            AddChild(_movement);
		}
	}
}
