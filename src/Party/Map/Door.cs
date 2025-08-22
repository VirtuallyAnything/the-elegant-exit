using Godot;

namespace tee
{
	public partial class Door : Interactable
	{
		private bool _isOpen;
		private bool _canBeUsed;
		private int _blockingBodies;
		private int _enemiesInRange;
		[Export] private Sprite2D _sprite;
		[Export] private NavigationRegion2D _navRegion;
		private Area2D _swingCone = new();
		private DynamicLightOccluder2D _lightOccluder = new();

		public override void _Ready()
		{
			base._Ready();
			Vector2 textureSize = _sprite.Texture.GetSize();
			Vector2 center = textureSize / 2;

			float radius = textureSize.X;
			_triggerArea.Position = center;

			CollisionCone collisionCone = new()
			{
				Radius = radius,
				AngleRadians = Mathf.DegToRad(90),
				Segments = 5
			};
			collisionCone.RotationDegrees -= 45;
			_swingCone.AddChild(collisionCone);
			_swingCone.BodyEntered += OnSwingConeEntered;
			_swingCone.BodyExited += OnSwingConeExited;
			AddChild(_swingCone);

			_lightOccluder.Occluder = new OccluderPolygon2D()
			{
				Polygon = textureSize.ToVertices(_sprite.Position, false)
			};
			_lightOccluder.AddToGroup("Occluder", true);
			_sprite.AddChild(_lightOccluder);
		}

		public override void _Input(InputEvent @event)
		{
			if (!_canBeUsed)
			{
				return;
			}
			if (Input.IsActionJustPressed("Interact"))
			{
				if (_isOpen && _blockingBodies != 0 || _enemiesInRange != 0)
				{
					return;
				}
				_sprite.RotationDegrees = _isOpen ? 0 : -90;
				_navRegion.Enabled = !_navRegion.Enabled;
				_isOpen = !_isOpen;
				_lightOccluder.OnRotationChanged();
			}
		}

		protected override void OnTriggerAreaEntered(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_canBeUsed = true;
			}
			else if (body.IsInGroup("Enemy"))
			{
				if (!_isOpen)
				{
					_sprite.RotationDegrees = -90;
					_isOpen = true;
					_navRegion.Enabled = true;
				}
				_enemiesInRange++;
			}
		}

		protected override void OnTriggerAreaExited(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_canBeUsed = false;
			}
			else if (body.IsInGroup("Enemy"))
			{
				_enemiesInRange--;
			}
		}

		private void OnSwingConeEntered(Node2D body)
		{
			if (body.IsInGroup("Enemy") || body.IsInGroup("Player"))
			{
				_blockingBodies++;
			}

		}

		private void OnSwingConeExited(Node2D body)
		{
			if (body.IsInGroup("Enemy") || body.IsInGroup("Player"))
			{
				_blockingBodies--;
			}
		}
	}
}
