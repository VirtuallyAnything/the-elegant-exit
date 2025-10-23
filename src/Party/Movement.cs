using Godot;
using System;

namespace tee
{
	public delegate void MovementHandler(Vector2 position, float rotation);
	public partial class Movement : Node
	{
		[Export] protected NavigationAgent2D _navAgent;
		[Export] protected Node2D _nodeToMove;
		private Vector2 _movementVector;
		[Export] private float _speed = 100;
		[Export] protected Node2D _nodeToRotate;
		[Export] protected float _turnSpeed = (float)Math.Tau;
		private float _movementDelta;
		private Vector2 _safeVelocity;
		public float Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}
		public float TurnSpeed
		{
			get { return _turnSpeed; }
			set { _turnSpeed = value; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_navAgent.VelocityComputed += OnNavigationAgent2DVelocityComputed;
		}

		protected virtual void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
		{
			_nodeToMove.GlobalPosition = _nodeToMove.GlobalPosition.MoveToward(_nodeToMove.GlobalPosition + safeVelocity, _movementDelta);
		}

		public override void _PhysicsProcess(double delta)
		{
			if (!_navAgent.IsNavigationFinished())
			{
				_movementDelta = _speed * (float)delta;
				Vector2 nextPathPosition = _navAgent.GetNextPathPosition();
				float targetAngle = _nodeToMove.GlobalPosition.DirectionTo(nextPathPosition).Angle();
				float angleDiff = (float)Mathf.Wrap(targetAngle - _nodeToRotate.Rotation, -Math.PI, Math.PI);
				_nodeToRotate.Rotation += (float)Math.Clamp(delta * _turnSpeed, 0, Math.Abs(angleDiff)) * Math.Sign(angleDiff);
				Vector2 newVelocity = _nodeToMove.GlobalPosition.DirectionTo(nextPathPosition) * _movementDelta;
				if (_navAgent.AvoidanceEnabled)
				{
					_navAgent.Velocity = newVelocity;
				}
				else
				{
					OnNavigationAgent2DVelocityComputed(newVelocity);
				}
			}
		}

		public override void _ExitTree()
		{
			_navAgent.VelocityComputed -= OnNavigationAgent2DVelocityComputed;
		}
	}
}
