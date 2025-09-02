using Godot;
using System;

namespace tee
{
	public partial class InteractiveArea : CircleTrigger, ITriggerActivator, ITriggerDeactivator
	{
		[Export] ShaderMaterial _activeShader;
		[Export] private float _outlineWidth;
		public override void _Ready()
		{
			_activeShader.SetShaderParameter("outline_width", _outlineWidth);
		}
		
		public void OnTriggerAreaEntered(Node2D body)
        {
            throw new NotImplementedException();
        }

        public void OnTriggerAreaExited(Node2D body)
        {
            throw new NotImplementedException();
        }
	}
}
