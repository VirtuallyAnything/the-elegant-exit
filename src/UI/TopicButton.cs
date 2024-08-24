using Godot;
using System;

namespace tee
{

	public delegate void TopicButtonHandler();
	public partial class TopicButton : Button
	{
		public static event TopicButtonHandler OnButtonPressed;
		private TopicName _conversationTopic;
		private AttackCardBack _parent;
		public AttackCardBack Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
		public TopicName ConversationTopic
		{
			get { return _conversationTopic; }
			set { _conversationTopic = value; }
		}

		public override void _Ready()
		{
			Pressed += OnPressed;
		}

		public void OnPressed()
		{
			_parent.ChildPressed(this);
			OnButtonPressed?.Invoke();
		}
	}
}
