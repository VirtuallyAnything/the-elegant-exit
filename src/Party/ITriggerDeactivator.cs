using System;
using Godot;

namespace tee
{
    public interface ITriggerDeactivator
    {
        public void OnTriggerAreaExited(Node2D body);
    }  
}
