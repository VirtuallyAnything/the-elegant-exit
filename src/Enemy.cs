using Godot;
using System;

namespace tee
{
	public partial class Enemy : Node2D
	{
		[Export] private float _triggerRange;
		//Some Variable for line of sight maybe?
		[Export] private EnemyData _enemyData;
		private Sprite2D _sprite = new Sprite2D();
		Area2D _triggerArea = new Area2D();


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CollisionShape2D collisionShape = new CollisionShape2D();
			CircleShape2D circle = new CircleShape2D();
			circle.Radius = _triggerRange;
			collisionShape.Shape = circle;
			_triggerArea.AddChild(collisionShape);
			_triggerArea.BodyEntered += OnBodyEntered;

			_sprite.Texture = _enemyData.Icon;
			AddChild(_sprite);
			AddChild(_triggerArea);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public override void _ExitTree()
		{
			_triggerArea.BodyEntered -= OnBodyEntered;
		}

		private void OnBodyEntered(Node2D body)
		{
			GD.Print($"Enemy {_enemyData.DisplayName} triggers fight");
			SceneManager.ChangeToEncounterScene(_enemyData);
		}
	}
}
