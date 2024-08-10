using Godot;
using System;

namespace tee
{

	public partial class Player : Node2D
	{
		[Export] private PlayerData _data;
		[Export] private int _sightConeSegments;
		[Export] private float _sightConeAngleDegrees;
		[Export] private float _sightConeRadius;
		[Export] private NavigationAgent2D _agent;
		[Export] private float _movementSpeed;

		[Export] private PointLight2D _discoveryLight;
		private PlayerVision _playerVision;
		private PlayerMovement _movement;
		public PlayerData Data{
			get{return _data;}
		}
		public PlayerMovement Movement{
			get{return _movement;}
		}

		public override void _Ready()
		{
			CollisionCone collisionCone = new()
			{
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};
			RemoveChild(_discoveryLight);
			_playerVision = new(_discoveryLight);
			_playerVision.AddChild(collisionCone);
			AddChild(_playerVision);

            _movement = new(_agent, this, _playerVision)
            {
                Speed = _movementSpeed
            };
            AddChild(_movement);
		}
	}
}
