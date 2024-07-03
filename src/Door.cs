using Godot;
using System;
using System.ComponentModel;

namespace tee
{
	public partial class Door : TextureRect
	{
		private bool _isOpen;

		public override void _GuiInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton input)
			{
				AcceptEvent();
				if (!input.Pressed)
				{
					RotationDegrees =_isOpen ? 0 : -90;
					_isOpen = !_isOpen;	
				}
			}
		}
	}
}
