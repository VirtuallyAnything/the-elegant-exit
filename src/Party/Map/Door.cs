using Godot;

namespace tee
{
	public partial class Door : OccludingPartyObject, ITriggerActivator, ITriggerDeactivator
	{
		private bool _isOpen;
		private bool _canBeUsed;
		private int _blockingBodies;
		private int _enemiesInRange;
		private CircleTrigger _trigger;
		[Export] private Sprite2D _sprite;
		[Export] private NavigationRegion2D _navRegion;
		[Export] ShaderMaterial _activeShader;
		[Export] private float _outlineWidth;
		private Area2D _swingCone = new();

		public override void _Ready()
		{
			base._Ready();
			_trigger = new()
			{
				Range = 100
			};
			Vector2 textureSize = _sprite.Texture.GetSize();
			Vector2 center = textureSize / 2;

			float radius = textureSize.X;
			_trigger.Position = center;
			AddChild(_trigger);

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

			_activeShader.SetShaderParameter("outline_width", _outlineWidth);
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
				RotationDegrees = _isOpen ? 0 : -90;
				_navRegion.Enabled = !_navRegion.Enabled;
				_isOpen = !_isOpen;
			}
		}

		public void OnTriggerAreaEntered(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_canBeUsed = true;
				_sprite.Material = _activeShader;
			}
			else if (body.IsInGroup("Enemy"))
			{
				if (!_isOpen)
				{
					RotationDegrees = -90;
					_isOpen = true;
					_navRegion.Enabled = true;
				}
				_enemiesInRange++;
			}
		}

		public void OnTriggerAreaExited(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_canBeUsed = false;
				_sprite.Material = null;
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

		public override void _ExitTree()
		{
			base._ExitTree();
			_swingCone.BodyEntered -= OnSwingConeEntered;
			_swingCone.BodyExited -= OnSwingConeExited;
		}
	}
}
