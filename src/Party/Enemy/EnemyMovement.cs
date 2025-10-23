using Godot;

namespace tee
{
	public partial class EnemyMovement : Movement
	{
		[Export] private Godot.Collections.Array<Node2D> _patrolPath = new();
		private int _patrolPathIndex;
		[Export] private bool _isPatrolCyclic;
		private PartyPlayer _player;
		[Export] private float _playerFollowSeconds = 3;
		private double _playerFollowSecondsLeft;
		private float _correctionMargin = 30;
		private Vector2 _lastPlayerPosition;
		private bool _isSeeingPlayer;
		private bool _isPlayerInSightCone;
		[Export] private EnemyVision _enemyVision;

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
				Patrol();
			}
		}

		private void Patrol()
		{
			if(_patrolPath.Count == 0)
            {
				_navAgent.TargetPosition = _nodeToMove.GlobalPosition;
				return;
            }
            if (_navAgent.IsNavigationFinished())
            {
				_navAgent.TargetPosition = _patrolPath[_patrolPathIndex].GlobalPosition;
				if(_patrolPathIndex < _patrolPath.Count - 1)
                {
					_patrolPathIndex++;
                }else if(_isPatrolCyclic)
                {
					_patrolPathIndex = 0;
                }
                else
                {
					_patrolPathIndex--;
                }
            }	
        }
	}
}
