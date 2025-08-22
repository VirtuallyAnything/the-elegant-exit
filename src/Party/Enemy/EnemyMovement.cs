using Godot;
using System;

namespace tee
{
	public partial class EnemyMovement : Movement
	{
		private PartyPlayer _player;
		private float _playerFollowSeconds = 10;
		private double _playerFollowSecondsLeft;
		private float _correctionMargin = 30;
		private Vector2 _lastPlayerPosition;
		private bool _isSeeingPlayer;
		private bool _isPlayerInSightCone;
		private EnemyVision _enemyVision;

		public PartyPlayer Player
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

		public EnemyMovement(NavigationAgent2D navAgent, Node2D nodeToMove, EnemyVision enemyVision) : base(navAgent, nodeToMove, enemyVision)
		{
			_enemyVision = enemyVision;
		}

		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double delta)
		{
			if (_enemyVision.IsSeeingPlayer())
			{
				_playerFollowSecondsLeft = _playerFollowSeconds;
			}
			else
			{
				_playerFollowSecondsLeft -= delta;
			}

			if (_playerFollowSecondsLeft > 0)
			{
				Vector2 currentPlayerPosition = _enemyVision.CurrentPlayerPosition;


				if (_lastPlayerPosition.DistanceTo(currentPlayerPosition) > _correctionMargin)
				{
					_navAgent.TargetPosition = currentPlayerPosition;
				}
			}
			else
			{
				_navAgent.TargetPosition = _nodeToMove.GlobalPosition;
			}
		}
	}
}
