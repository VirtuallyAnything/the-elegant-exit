using Godot;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace tee
{
	public partial class CircleTrigger : Area2D
	{
		[Export] private float _range = 10;
		private Node _receiver;
		private BodyEnteredEventHandler _callOnBodyEntered;
		private BodyExitedEventHandler _callOnBodyExited;
		public float Range
		{
			get { return _range; }
			set { _range = value; }
		}
		public Node Receiver
		{
			get { return _receiver; }
			set
			{
				if (_receiver is not null)
				{
					DisconnectAllDelegates();
				}

				_receiver = value;
				if (_receiver is ITriggerActivator newActivator)
				{
					_callOnBodyEntered = newActivator.OnTriggerAreaEntered;
					BodyEntered += _callOnBodyEntered;
				}

				if (_receiver is ITriggerDeactivator newDeactivator)
				{
					_callOnBodyExited = newDeactivator.OnTriggerAreaExited;
					BodyExited += _callOnBodyExited;
				}
			}
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
		}

		public void DisconnectAllDelegates()
		{
			if (_callOnBodyEntered is not null)
			{
				BodyEntered -= _callOnBodyEntered;
				_callOnBodyEntered = null;
			}

			if (_callOnBodyExited is not null)
			{
				BodyExited -= _callOnBodyExited;
				_callOnBodyExited = null;
			}
		}

		public override void _ExitTree()
		{
			DisconnectAllDelegates();
		}
	}
}
