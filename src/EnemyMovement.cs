using Godot;
using System;

namespace tee
{

	public partial class EnemyMovement : Movement
	{
		private Player _player;
		private float _playerFollowSeconds = 10;
		private double _playerFollowSecondsLeft;
		private float _correctionMargin = 30;
		private Vector2 _lastPlayerPosition;
		private bool _isSeeingPlayer;
		private bool _isPlayerInSightCone;
		private EnemySight _enemySight;

		public Player PlayerReference
		{
			get { return _player; }
			set { _player = value; }
		}
		public float PlayerFollowSeconds
		{
			get { return _playerFollowSeconds; }
			set { _playerFollowSeconds = value; }
		}
		public float CorrectionMargin
		{
			get { return _correctionMargin; }
			set { _correctionMargin = value; }
		}

		public EnemyMovement(NavigationAgent2D navAgent, Node2D nodeToMove, EnemySight enemySight) : base(navAgent, nodeToMove)
		{
			_enemySight = enemySight;
		}

		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double delta)
		{
			if (_enemySight.IsSeeingPlayer())
			{
				_playerFollowSecondsLeft = _playerFollowSeconds;
			}
			else
			{
				_playerFollowSecondsLeft -= delta;
			}

			if (_playerFollowSecondsLeft > 0)
			{
				Vector2 currentPlayerPosition = _enemySight.CurrentPlayerPosition;


				if (_lastPlayerPosition.DistanceTo(currentPlayerPosition) > _correctionMargin)
				{
					NavigateTo(currentPlayerPosition, (float)delta);
				}
			}
			else
			{
				_navAgent.TargetPosition = _nodeToMove.GlobalPosition;
			}
		}

		private void NavigateTo(Vector2 position, float delta)
		{
			_navAgent.TargetPosition = position;
			float targetAngle = _nodeToMove.GlobalPosition.DirectionTo(position).Angle();
			float angleDiff = (float)Mathf.Wrap(targetAngle - _enemySight.Rotation, -Math.PI, Math.PI);
			_enemySight.Rotation += Math.Clamp(delta * _turnSpeed, 0, Math.Abs(angleDiff)) * Math.Sign(angleDiff);
		}
	}
}
