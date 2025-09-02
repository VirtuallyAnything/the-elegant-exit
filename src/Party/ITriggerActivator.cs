using System;
using Godot;

namespace tee
{
    public interface ITriggerActivator
    {
        public void OnTriggerAreaEntered(Node2D body);
    }  
}
