using Godot;
using System;
using System.Diagnostics;

namespace tee
{
	public partial class PartyEnemy : Interactable, IPlayerVisible
	{
		[Export] private EnemyData _enemyData;
		private CollisionShape2D _collisionShape;
		private EnemyVision _enemyVision;
		[Export] private int _sightConeSegments = 12;
		[Export] private float _sightConeAngleDegrees = 120;
		[Export] private float _sightConeRadius = 800;
		private Sprite2D _sprite;

		private EnemyMovement _enemyMovement;
		private RayCast2D _rayCast = new();
		private NavigationAgent2D _navAgent = new();
		[Export] private float _playerFollowSeconds = 3;
		[Export] private float _speed = 200;
		[Export] private float _turnSpeed = (float)Math.Tau;
		[Export] private float _startingRotationDeg;
		private bool _isInChase;

		private Tween _tween;
		private SceneManager _sceneManager;

        public void OnSightConeEntered()
        {
			this.AppearInView(_tween);
        }

		public void OnSightConeExited()
		{
			this.FadeFromView(_tween);
			// Maybe add a sprite at the last player-known location to symbolize where the enemy was last. 
			// Sprite is deleted as soon as it or the actual enemy is in player sight cone
        }

        public override void _Ready()
		{
			base._Ready();

			_sprite = new();
			_sprite.Texture = _enemyData.Icon;
			Modulate = new Color(1, 1, 1, 0);
			
			AddChild(_navAgent);

			CollisionCone collisionCone = new()
			{
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};
			_enemyVision = new(_rayCast);
			_enemyVision.AddChild(collisionCone);
			_enemyVision.RotationDegrees = _startingRotationDeg;

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

        protected override void OnTriggerAreaExited(Node2D body)
        {
            throw new NotImplementedException();
        }
    }
}
