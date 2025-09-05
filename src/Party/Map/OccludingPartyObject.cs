using Godot;
using System;

namespace tee
{
    public partial class OccludingPartyObject : Node2D, IDynamicallyVisible
    {
        private bool _isVisible = false;
        [Export] private LightOccluder2D _lightOccluder;

        public override void _Ready()
        {
            _lightOccluder.AddToGroup("Occluder", true);
            GD.Print("OccludingPartyObject: LightOccluder Instance ID: " + _lightOccluder.GetInstanceId());
            Modulate = new Color(1, 1, 1, 0);
        }

        public void OnSightConeEntered()
        {
            if (!_isVisible)
            {
                this.AppearInView(null);
                _isVisible = true;
            }
        }

        public void OnSightConeExited()
        {
        }
    }
}