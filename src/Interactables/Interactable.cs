using Godot;
using System;

namespace tee
{
	public partial class Interactable : Node2D
	{
		[Export] protected float _triggerRange;
		protected Sprite2D _sprite = new Sprite2D();
		Area2D _triggerArea = new Area2D();


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

			AddChild(_sprite);
			AddChild(_triggerArea);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		protected virtual void OnTriggerAreaEntered(Node2D body){

		}
		

		
	}
}
