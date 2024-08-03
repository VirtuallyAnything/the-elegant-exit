using Godot;
using System;

namespace tee
{
	public partial class EnemyVision : Vision
	{
		private RayCast2D _rayCast;
		private Player _player;
		private Vector2 _lastPlayerPosition;
		private bool _isRayHittingPlayer;
		private bool _isPlayerInSightCone;
		public Vector2 LastPlayerPosition{
			get{return _lastPlayerPosition;}
		}
		public Vector2 CurrentPlayerPosition{
			get{return _player.Position;}
		}

		// Called when the node enters the scene tree for the first time.
		public EnemyVision(RayCast2D raycast){
			_rayCast = raycast;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (_player is null)
			{
				return;
			}

			if (_isPlayerInSightCone)
			{
				_rayCast.TargetPosition = CurrentPlayerPosition - GlobalPosition;
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

		protected override void OnSightConeEntered(Node2D body)
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

		protected override void OnSightConeExited(Node2D body)
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
