using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
namespace tee
{
	public partial class PlayerMovement : Movement
	{
		public static event MovementHandler NodeMoved;

		public PlayerMovement(NavigationAgent2D navAgent, Node2D nodeToMove, Node2D nodeToRotate = null) : base(navAgent, nodeToMove, nodeToRotate)
		{
		}

		protected override void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
		{
			base.OnNavigationAgent2DVelocityComputed(safeVelocity);
		}

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
			if(!_navAgent.IsNavigationFinished()){
				NodeMoved?.Invoke(_nodeToMove.GlobalPosition);
			}
        }

        public override void _UnhandledInput(InputEvent @event)
		{
			if (Input.IsActionPressed("Move"))
			{
				if (@event is InputEventMouseButton eventMouseButton)
				{
					Vector2 targetPosition = GetViewport().CanvasTransform.AffineInverse() * eventMouseButton.Position;
					_navAgent.TargetPosition = targetPosition;
				}
			}
		}
	}
}
