using System;
using Godot;

namespace tee
{
    public interface IDynamicallyVisible
    {
        public void OnSightConeEntered();
        public void OnSightConeExited();
    }  
}
