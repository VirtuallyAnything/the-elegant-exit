using Godot;
using System;

namespace tee
{
	public partial class Enemy : Interactable
	{
		//Some Variable for line of sight maybe?
		[Export] private EnemyData _enemyData;

		private SceneManager _sceneManager;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_sprite.Texture = _enemyData.Icon;
			_sceneManager = GetNode("/root/SceneManager") as SceneManager;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		override protected void OnBodyEntered(Node2D body)
		{
			GD.Print($"Enemy {_enemyData.DisplayName} triggers fight");
			GameManager.CurrentEnemy = _enemyData;
			_sceneManager.ChangeToScene(SceneName.EncounterStart);
			QueueFree();
		}

		
	}
}
