using Godot;
using System;
using System.ComponentModel;

namespace tee
{
	public partial class Door : Interactable
	{
		private bool _isOpen;
		private bool _canBeUsed;
		private int _blockingBodies;
		private int _enemiesInRange;
		private Texture2D _texture;
		[Export] private NavigationRegion2D _navRegion = new();
		private Area2D _swingCone = new();

		public override void _Ready()
		{
			base._Ready();
			_texture = GD.Load<Texture2D>("res://Assets/Textures/door.png");
			_sprite.Texture = _texture;
			_sprite.Scale = new Vector2(1.487f, 1.487f);
			Vector2 textureSize = _texture.GetSize();
			Vector2 center = textureSize / 2;

			float radius = textureSize.X;
			_triggerArea.Position = center;
			_sprite.Offset = center;

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
