using Godot;
using System;
using System.Diagnostics;

namespace tee
{
	public partial class PartyEnemy : PartyCharacter, IDynamicallyVisible, ITriggerActivator
	{
		[Export] private EnemyData _enemyData;
		private CircleTrigger _trigger = new();
		private CollisionShape2D _collisionShape;
		private Sprite2D _sprite;
		private RayCast2D _rayCast = new();
		private NavigationAgent2D _navAgent = new();
		[Export] private float _playerFollowSeconds = 3;
		[Export] private float _turnSpeed = (float)Math.Tau;
		[Export] private float _startingRotationDeg;
		private bool _isInChase;
		private Tween _tween;
		private SceneManager _sceneManager;

		public override void _Ready()
		{
			_sprite = new();
			_sprite.Texture = _enemyData.Icon;
			Modulate = new Color(1, 1, 1, 0);

			AddChild(_navAgent);

            _vision = new EnemyVision(_rayCast)
            {
                RotationDegrees = _startingRotationDeg
            };

            AddChild(_rayCast);

			_movement = new EnemyMovement(_navAgent, this, (EnemyVision)_vision)
			{
				PlayerFollowSeconds = _playerFollowSeconds,
				Speed = _movementSpeed,
				TurnSpeed = _turnSpeed
			};

			_collisionShape = new()
			{
				Shape = new CircleShape2D()
				{
					Radius = _sprite.Texture.GetSize().X / 2
				}
			};
			AddChild(_collisionShape);

			_sceneManager = GetNode("/root/SceneManager") as SceneManager;
			base._Ready();
		}

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

		public void OnTriggerAreaEntered(Node2D body)
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
