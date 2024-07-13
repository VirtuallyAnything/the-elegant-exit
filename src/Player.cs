using Godot;
using System;

namespace tee
{
	public partial class Player : Node2D
	{
		[Export] private PlayerData _data;
		[Export] private NavigationAgent2D _agent;
		private Movement _movement;

		public override void _Ready()
		{
			_movement = new(_agent, this);
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

		public void Attack(int attackNumber)
		{

		}
	}
}
