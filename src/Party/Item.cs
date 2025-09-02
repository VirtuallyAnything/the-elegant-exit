using Godot;
using System;

namespace tee
{
	public partial class Item : Node2D, ITriggerActivator, ITriggerDeactivator
	{
		[Export] private ItemData _itemData;
		private CircleTrigger _trigger = new();
		private Sprite2D _sprite;

		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _itemData.Texture;
		}

		public void OnTriggerAreaEntered(Node2D body)
		{

		}

        public void OnTriggerAreaExited(Node2D body)
        {
           // throw new NotImplementedException();
        }

    }
}
