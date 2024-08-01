using Godot;
using System;

namespace tee
{

	public partial class Player : Node2D
	{
		[Export] private PlayerData _data;
		[Export] private NavigationAgent2D _agent;
		[Export] private float _movementSpeed;
		[Export] private PointLight2D _playerVision;
		private PlayerMovement _movement;
		public PlayerData Data{
			get{return _data;}
		}
		public PlayerMovement Movement{
			get{return _movement;}
		}

		public override void _Ready()
		{
            _movement = new(_agent, this, _playerVision)
            {
                Speed = _movementSpeed
            };
            AddChild(_movement);
		}
	}
}
