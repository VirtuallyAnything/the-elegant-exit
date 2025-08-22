using Godot;
using System;

namespace tee
{
	public partial class Item : Interactable
	{
		[Export] private ItemData _itemData;
		private Sprite2D _sprite;

		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _itemData.Texture;
			_triggerRange = 100;
		}

		protected override void OnTriggerAreaEntered(Node2D body)
		{

		}

        protected override void OnTriggerAreaExited(Node2D body)
        {
           // throw new NotImplementedException();
        }

    }
}
