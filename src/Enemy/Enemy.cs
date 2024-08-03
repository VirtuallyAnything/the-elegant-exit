using Godot;
using System;

namespace tee
{
	public partial class Enemy : Interactable
	{
		[Export] private EnemyData _enemyData;
		private CollisionShape2D _collisionShape;
		private EnemyVision _enemyVision;
		[Export] int _sightConeSegments;
		[Export] float _sightConeAngleDegrees;
		[Export] float _sightConeRadius;

		private EnemyMovement _enemyMovement;
		private RayCast2D _rayCast = new();
		private NavigationAgent2D _navAgent = new();
		[Export] private float _playerFollowSeconds = 10;
		[Export] private float _speed;
		[Export] private float _turnSpeed = (float)Math.Tau;

		private bool _isVisibleToPlayer;
		private SceneManager _sceneManager;

		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _enemyData.Icon;
			AddChild(_navAgent);

			CollisionCone collisionCone = new()
			{
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};
			_enemyVision = new(_rayCast);
			_enemyVision.AddChild(collisionCone);

			AddChild(_enemyVision);
			AddChild(_rayCast);

			_enemyMovement = new(_navAgent, this, _enemyVision){
				PlayerFollowSeconds = _playerFollowSeconds,
				Speed = _speed,
				TurnSpeed = _turnSpeed
			};
			AddChild(_enemyMovement);

			_collisionShape = new()
			{
				Shape = new CircleShape2D()
				{
					Radius = _sprite.Texture.GetSize().X / 2
				}
			};
			AddChild(_collisionShape);

			_sceneManager = GetNode("/root/SceneManager") as SceneManager;
		}

		override protected void OnTriggerAreaEntered(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				GD.Print($"Enemy {_enemyData.DisplayName} triggers fight");
				GameManager.CurrentEnemy = _enemyData;
				_sceneManager.ChangeToScene(SceneName.EncounterStart);
				QueueFree();
			}
		}

	}
}
