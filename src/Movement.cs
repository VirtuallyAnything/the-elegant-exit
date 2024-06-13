using Godot;
using System;
using System.Diagnostics;

public partial class Movement : CharacterBody2D
{
	[Export] private NavigationAgent2D _agent;
	[Export] private Camera2D _camera;
	private Vector2 _movementVector;
	private float _movementSpeed = 100;
	private float _movementDelta;
	private Vector2 _safeVelocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_agent.VelocityComputed += OnNavigationAgent2DVelocityComputed;
	}

    private void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
    {
        GlobalPosition = GlobalPosition.MoveToward(GlobalPosition + safeVelocity, _movementDelta);
    }

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionPressed("Move"))
		{
			// _movementVector = Input.GetVector("move_forwards", "move_backwards", "move_left", "move_right");
			if (@event is InputEventMouseButton eventMouseButton){
				_agent.TargetPosition = GetViewport().CanvasTransform.AffineInverse() * eventMouseButton.Position;
			}
		}
	}

	public override void _PhysicsProcess(double delta){
		if(!_agent.IsNavigationFinished()){
			_movementDelta = _movementSpeed * (float) delta;
			Vector2 nextPathPosition = _agent.GetNextPathPosition();
			Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * _movementDelta;
			if (_agent.AvoidanceEnabled){
				_agent.Velocity = newVelocity;
			}else{
				OnNavigationAgent2DVelocityComputed(newVelocity);
			}
				//GlobalPosition = GlobalPosition.MoveToward(_agent.GetNextPathPosition(), (float)delta * _movementSpeed);
			//GD.Print($"Speed: {delta * _safeSpeed}");
		}
	}
	
}
