using Godot;
using System;
using System.Diagnostics;

public partial class Movement : Node
{
	protected NavigationAgent2D _navAgent;
	protected Node2D _nodeToMove;
	private Vector2 _movementVector;
	private float _speed = 100;
	protected float _turnSpeed = (float)Math.Tau;
	private float _movementDelta;
	private Vector2 _safeVelocity;
	public float Speed{
		get{return _speed;}
		set{_speed = value;}
	}
	public float TurnSpeed{
		get{return _turnSpeed;}
		set{_turnSpeed = value;}
	}

	public Movement(NavigationAgent2D navAgent, Node2D nodeToMove)
	{
		_navAgent = navAgent;
		_nodeToMove = nodeToMove;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_navAgent.VelocityComputed += OnNavigationAgent2DVelocityComputed;
	}

	private void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
	{
		_nodeToMove.GlobalPosition = _nodeToMove.GlobalPosition.MoveToward(_nodeToMove.GlobalPosition + safeVelocity, _movementDelta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_navAgent.IsNavigationFinished())
		{
			_movementDelta = _speed * (float)delta;
			Vector2 nextPathPosition = _navAgent.GetNextPathPosition();
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
