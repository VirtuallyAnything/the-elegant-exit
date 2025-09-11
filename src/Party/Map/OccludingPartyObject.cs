using Godot;
using System;

namespace tee
{
    /// <summary>
    /// Script to attach to an object that obscurs Player Vision.
    /// </summary>
    public partial class OccludingPartyObject : StaticBody2D, IDynamicallyVisible
    {
        private bool _isVisible = false;
        [Export] private LightOccluder2D _lightOccluder;

        public override void _Ready()
        {
            _lightOccluder.AddToGroup("Occluder", true);
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