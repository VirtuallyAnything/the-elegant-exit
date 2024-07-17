using Godot;
using System;

namespace tee
{
	public partial class Item : Interactable
	{
		[Export] private ItemData _itemData;

		public override void _Ready()
		{
			base._Ready();
			_sprite.Texture = _itemData.Texture;
			_triggerRange = 100;
		}

        protected override void OnTriggerAreaEntered(Node2D body)
        {

        }
    }
}
