using Godot;
using System;

namespace tee
{
	public partial class EnemySight : Area2D
	{
		private RayCast2D _rayCast;
		private Player _player;
		private Vector2 _lastPlayerPosition;
		private bool _isRayHittingPlayer;
		private bool _isPlayerInSightCone;

		// Called when the node enters the scene tree for the first time.
		public EnemySight(RayCast2D raycast){
			_rayCast = raycast;
			BodyEntered += OnSightConeEntered;
			BodyExited += OnSightConeExited;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (_player is null)
			{
				return;
			}

			Vector2 currentPlayerPosition = _player.Position;
			if (_isPlayerInSightCone)
			{
				_rayCast.TargetPosition = currentPlayerPosition - GlobalPosition;
				if (_rayCast.GetCollider() is not Player)
				{
					_isRayHittingPlayer = false;
				}
				else
				{
					_isRayHittingPlayer = true;
				}
			}
		}

		public void CheckLineOfSightToPlayer()
		{
			if(_player is null){
				GD.PrintErr("EnemySight Player Reference is null.");
				return;
			}
			_rayCast.TargetPosition = _player.Position - GlobalPosition;
			if (_rayCast.GetCollider() is not Player)
			{
				_isRayHittingPlayer = false;
			}
			else if (_isPlayerInSightCone)
			{
				_isRayHittingPlayer = true;
				
			}
		}

		private void OnSightConeEntered(Node2D body)
		{
			
			if (body is Player player)
			{
				_rayCast.Enabled = true;
				_isPlayerInSightCone = true;
				_player = player;
				_lastPlayerPosition = player.Position;
				_rayCast.TargetPosition = _lastPlayerPosition - GlobalPosition;
				_rayCast.ForceRaycastUpdate();
				if (_rayCast.GetCollider() is not Player)
				{
					_isRayHittingPlayer = false;
					return;
				}
				_isRayHittingPlayer = true;	
			}
		}

		private void OnSightConeExited(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_rayCast.Enabled = false;
				_isPlayerInSightCone = false;
			}
		}

		public bool IsSeeingPlayer(){
			return _isPlayerInSightCone && _isRayHittingPlayer;
		}
	}
}
