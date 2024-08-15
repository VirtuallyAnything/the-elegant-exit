using Godot;
using System;

namespace tee
{
	public partial class PlayerVision : Vision
	{
		private PointLight2D _discoveryLight;

		public PlayerVision(PointLight2D discoveryLight){
			_discoveryLight = discoveryLight;
		}

        public override void _Ready()
        {
            base._Ready();
			AddChild(_discoveryLight);
        }

        protected override void OnSightConeEntered(Node2D body)
        {
            if(body is PartyEnemy enemy){
				enemy.AppearInView();
			}
        }

        protected override void OnSightConeExited(Node2D body)
        {
            if(body is PartyEnemy enemy){
				enemy.FadeFromView();
			}
        }
    }
}