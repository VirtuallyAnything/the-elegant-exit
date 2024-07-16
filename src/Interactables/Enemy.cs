using Godot;
using System;

namespace tee
{
	public partial class Enemy : Interactable
	{
		//Some Variable for line of sight maybe?
		[Export] private EnemyData _enemyData;
		private Area2D _sightCone = new();
		[Export] int _sightConeSegments;
		[Export] float _sightConeAngleDegrees;
		[Export] float _sightConeRadius;
		private Movement _movement;
		private NavigationAgent2D _navAgent = new();
		private Node2D _player;
		private Vector2 _playerPosition;
		private bool _isSeeingPlayer;
		private bool _isPlayerInSightCone;
		private RayCast2D _rayCast = new();
		[Export] private float _playerFollowSeconds = 10;
		private double _playerFollowSecondsLeft;
		[Export] private float _turnSpeed = (float)Math.Tau;
		private SceneManager _sceneManager;

		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _enemyData.Icon;
			AddChild(_navAgent);
			_movement = new(_navAgent, this);
			AddChild(_movement);

			CollisionCone collisionCone = new()
			{
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};
			_sightCone.AddChild(collisionCone);
			_sightCone.BodyEntered += OnSightConeEntered;
			_sightCone.BodyExited += OnSightConeExited;
			AddChild(_sightCone);

			AddChild(_rayCast);

			_sceneManager = GetNode("/root/SceneManager") as SceneManager;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (_player == null)
			{
				return;
			}

			Vector2 currentPlayerPosition = _player.Position;
			if (_playerFollowSecondsLeft > 0)
			{

				if (_playerPosition.DistanceTo(currentPlayerPosition) > 10)
				{
					CheckLineOfSightToPlayer(currentPlayerPosition);

					_navAgent.TargetPosition = currentPlayerPosition;
					float targetAngle = GlobalPosition.DirectionTo(currentPlayerPosition).Angle();
					float angleDiff = (float)Mathf.Wrap(targetAngle - _sightCone.Rotation, -Math.PI, Math.PI);
					_sightCone.Rotation += Math.Clamp((float)delta * _turnSpeed, 0, Math.Abs(angleDiff)) * Math.Sign(angleDiff);
				}

				if (!_isSeeingPlayer)
				{
					_playerFollowSecondsLeft -= delta;
				}
			}else{
				_navAgent.TargetPosition = GlobalPosition;
			}

			if (_isPlayerInSightCone)
			{
				_rayCast.TargetPosition = currentPlayerPosition - Position;
				if (_rayCast.GetCollider() is not Player)
				{
					_isSeeingPlayer = false;
				}
				else
				{
					_isSeeingPlayer = true;
					_playerFollowSecondsLeft = _playerFollowSeconds;
				}
			}
		}

		override protected void OnTriggerAreaEntered(Node2D body)
		{
			GD.Print($"Enemy {_enemyData.DisplayName} triggers fight");
			GameManager.CurrentEnemy = _enemyData;
			_sceneManager.ChangeToScene(SceneName.EncounterStart);
			QueueFree();
		}

		private void OnSightConeEntered(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_player = body;
				_playerPosition = body.Position;
				_rayCast.TargetPosition = _playerPosition - Position;
				_rayCast.ForceRaycastUpdate();
				if (_rayCast.GetCollider() is not Player)
				{
					_isSeeingPlayer = false;
					_isPlayerInSightCone = true;
					return;
				}
				_isSeeingPlayer = true;
				_isPlayerInSightCone = true;
				_navAgent.TargetPosition = _playerPosition;
				_playerFollowSecondsLeft = _playerFollowSeconds;
			}
		}

		private void OnSightConeExited(Node2D body)
		{
			_isSeeingPlayer = false;
			_isPlayerInSightCone = false;
		}

		private void CheckLineOfSightToPlayer(Vector2 playerPosition)
		{
			_rayCast.TargetPosition = playerPosition - Position;
			if (_rayCast.GetCollider() is not Player)
			{
				_isSeeingPlayer = false;
			}
			else if (_isPlayerInSightCone)
			{
				_isSeeingPlayer = true;
				_playerFollowSecondsLeft = _playerFollowSeconds;
			}
		}
	}
}
