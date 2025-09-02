using Godot;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace tee
{
	public partial class CircleTrigger : Area2D
	{
		[Export] private float _range = 10;
		private Node _parent;
		private Callable _activatorMethod;
		public float Range
		{
			get { return _range; }
			set { _range = value; }
		}

		public override void _Ready()
		{
			CircleShape2D circle = new()
			{
				Radius = _range
			};
			CollisionShape2D collisionShape = new()
			{
				Shape = circle
			};
			AddChild(collisionShape);

			_parent = GetParent();
			if (_parent is ITriggerActivator activator)
			{
				BodyEntered += activator.OnTriggerAreaEntered;
			}

			if (_parent is ITriggerDeactivator deactivator)
			{
				BodyExited += deactivator.OnTriggerAreaExited;
			}
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			if (_parent is ITriggerActivator activator)
			{
				BodyEntered -= activator.OnTriggerAreaEntered;
			}

			if (_parent is ITriggerDeactivator deactivator)
			{
				BodyExited -= deactivator.OnTriggerAreaExited;
			}
		}
	}
}
