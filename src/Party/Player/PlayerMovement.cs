using Godot;
namespace tee
{
	public partial class PlayerMovement : Movement
	{
		public static event MovementHandler NodeMoved;

		protected override void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
		{
			base.OnNavigationAgent2DVelocityComputed(safeVelocity);
		}

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
			if(!_navAgent.IsNavigationFinished()){
				NodeMoved?.Invoke(_nodeToMove.GlobalPosition, _nodeToRotate.Rotation);
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
