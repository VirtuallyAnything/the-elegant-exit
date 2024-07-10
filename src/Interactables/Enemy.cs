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
		private NavigationAgent2D _navAgent = new();

		private SceneManager _sceneManager;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _enemyData.Icon;
			AddChild(_navAgent);

			CollisionCone collisionCone = new(){
				Segments = _sightConeSegments,
				AngleRadians = (float)(_sightConeAngleDegrees * (Math.PI / 180)),
				Radius = _sightConeRadius
			};
			_sightCone.AddChild(collisionCone);
			_sightCone.BodyEntered += OnSightConeEntered;
			AddChild(_sightCone);
			
			_sceneManager = GetNode("/root/SceneManager") as SceneManager;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		override protected void OnTriggerAreaEntered(Node2D body)
		{
			GD.Print($"Enemy {_enemyData.DisplayName} triggers fight");
			GameManager.CurrentEnemy = _enemyData;
			_sceneManager.ChangeToScene(SceneName.EncounterStart);
			QueueFree();
		}

		private void OnSightConeEntered(Node2D body){
			if (body.IsInGroup("Player")){

			}
		}

		
	}
}
