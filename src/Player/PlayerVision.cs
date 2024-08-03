using Godot;
using System;

namespace tee
{
	public partial class PlayerVision : Vision
	{
		[Export] private PointLight2D _discoveryLight;

		public PlayerVision(PointLight2D discoveryLight){
			_discoveryLight = discoveryLight;
		}

        protected override void OnSightConeEntered(Node2D body)
        {
            if(body is Enemy){
				body.Visible = true;
			}
        }

        protected override void OnSightConeExited(Node2D body)
        {
            if(body is Enemy){
				body.Visible = false;
			}
        }
    }
}