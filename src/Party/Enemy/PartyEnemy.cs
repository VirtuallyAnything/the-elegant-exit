using Godot;

namespace tee
{
	public partial class PartyEnemy : PartyCharacter, IDynamicallyVisible, ITriggerActivator
	{
		[Export] private EnemyData _enemyData;
		private CircleTrigger _trigger;
		private CollisionShape2D _collisionShape;
		[Export] private Sprite2D _sprite;
		private Tween _tween;
		private SceneManager _sceneManager;
		public override void _Ready()
		{
			_sprite.Texture = _enemyData.Icon;
			Modulate = new Color(1, 1, 1, 0);

			_trigger = new()
			{
				Range = 50,
				Receiver = this
			};
			AddChild(_trigger);

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
			_tween = this.FadeFromView(_tween);
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
