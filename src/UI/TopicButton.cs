using Godot;
using System;

namespace tee
{

	public delegate void TopicButtonHandler();
	public delegate void TopicButtonHoverHandler(TopicName topicName);
	public partial class TopicButton : Button
	{
		public static event TopicButtonHandler OnButtonPressed;
		public static event TopicButtonHoverHandler OnButtonHovered;
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
			MouseEntered += OnHover;
		}

		public void OnPressed()
		{
			_parent.ChildPressed(this);
			OnButtonPressed?.Invoke();
		}

		public void OnHover()
		{
			OnButtonHovered?.Invoke(_conversationTopic);
		}

		public override void _Notification(int what)
		{
			if (what == NotificationPredelete)
			{
				Pressed -= OnPressed;
				MouseEntered -= OnHover;
			}
		}

	}
}
