using Godot;
using System;
using System.Diagnostics;

namespace tee
{
	public delegate void MovementHandler(Vector2 position);
	public partial class Movement : Node
	{
		protected NavigationAgent2D _navAgent;
		protected Node2D _nodeToMove;
		private Vector2 _movementVector;
		private float _speed = 100;
		protected Node2D _nodeToRotate;
		protected float _turnSpeed = (float)Math.Tau;
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

		public Movement(NavigationAgent2D navAgent, Node2D nodeToMove, Node2D nodeToRotate = null)
		{
			_navAgent = navAgent;
			_nodeToMove = nodeToMove;
			if(nodeToRotate is null){
				_nodeToRotate = nodeToMove;
			}else{
				_nodeToRotate = nodeToRotate;
			}

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
	}
}
