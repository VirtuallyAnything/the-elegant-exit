using Godot;
using System;
using System.Diagnostics;

namespace tee
{
	public partial class PartyEnemy : Interactable
	{
		[Export] private EnemyData _enemyData;
		private CollisionShape2D _collisionShape;
		private EnemyVision _enemyVision;
		[Export] int _sightConeSegments = 12;
		[Export] float _sightConeAngleDegrees = 120;
		[Export] float _sightConeRadius = 800;

		private EnemyMovement _enemyMovement;
		private RayCast2D _rayCast = new();
		private NavigationAgent2D _navAgent = new();
		[Export] private float _playerFollowSeconds = 3;
		[Export] private float _speed = 200;
		[Export] private float _turnSpeed = (float)Math.Tau;
		[Export] private float _startingRotationDeg;
		private bool _isInChase;

		private Tween _tween;
		private Color _transparent = new Color(1, 1, 1, 0);
		private SceneManager _sceneManager;

		public override void _Ready()
		{
			base._Ready();
			//Modulate = _transparent;
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

		public void FadeFromView(){
			_tween = CreateTween();
			float animationLength = 2f;
			Color currentColor = Modulate;
			PropertyTweener propTweener = _tween.TweenProperty(
				this, $"{PropertyName.Modulate}", _transparent, animationLength);
			propTweener.From(currentColor);
		}

		public void AppearInView(){
			_tween?.Kill();
			Modulate = Color.Color8(255, 255, 255, 255);
		}

	}
}
