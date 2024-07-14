using Godot;
using System;

namespace tee
{
	public partial class Player : Node2D
	{
		[Export] private PlayerData _data;
		[Export] private NavigationAgent2D _agent;
		[Export] private float _movementSpeed;
		private Movement _movement;

		public override void _Ready()
		{
			_movement = new(_agent, this);
			_movement.Speed = _movementSpeed;
			AddChild(_movement);
		}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionPressed("Move"))
		{
			if (@event is InputEventMouseButton eventMouseButton)
			{
				_agent.TargetPosition = GetViewport().CanvasTransform.AffineInverse() * eventMouseButton.Position;
			}
		}
	}

        public Vector2 GetPosition(){
			return GlobalPosition;
		}

        public void Attack(int attackNumber)
		{

		}
	}
}
