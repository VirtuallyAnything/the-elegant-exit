using Godot;
using System;

namespace tee
{
	public partial class PartyObject : Node2D, IPlayerVisible
	{
        public void OnSightConeEntered()
        {
			this.AppearInView(null);
        }

        public void OnSightConeExited()
        {
        }

        public override void _Ready()
		{
			Modulate = new Color(1, 1, 1, 0);
		}
	}
}