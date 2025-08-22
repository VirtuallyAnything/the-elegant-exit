using Godot;
using System;

namespace tee
{
	public abstract partial class Interactable : Node2D
	{
		[Export] protected float _triggerRange = 10;
		protected Area2D _triggerArea = new Area2D();

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CircleShape2D circle = new()
			{
				Radius = _triggerRange
			};
			CollisionShape2D collisionShape = new()
			{
				Shape = circle
			};
			_triggerArea.AddChild(collisionShape);
			_triggerArea.BodyEntered += OnTriggerAreaEntered;
			_triggerArea.BodyExited += OnTriggerAreaExited;

			AddChild(_triggerArea);
		}

		protected abstract void OnTriggerAreaEntered(Node2D body);

		protected abstract void OnTriggerAreaExited(Node2D body);
	}
}
