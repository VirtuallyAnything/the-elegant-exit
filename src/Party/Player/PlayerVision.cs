using Godot;
using System;

namespace tee
{
	public partial class PlayerVision : Vision
	{
        public override void _Ready()
        {
            base._Ready();
        }

        protected override void OnSightConeEntered(Node2D body)
        {
            if(body is IDynamicallyVisible playerVisibleObject){
				playerVisibleObject.OnSightConeEntered();
			}
        }

        protected override void OnSightConeExited(Node2D body)
        {
            if(body is IDynamicallyVisible playerVisibleObject){
				playerVisibleObject.OnSightConeExited();
			}
        }
    }
}