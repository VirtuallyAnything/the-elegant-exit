using Godot;
using System;

namespace tee
{
	public abstract partial class Vision : Area2D
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			BodyEntered += OnSightConeEntered;
			BodyExited += OnSightConeExited;
		}

		protected abstract void OnSightConeEntered(Node2D body);

		protected abstract void OnSightConeExited(Node2D body);
	}
}
