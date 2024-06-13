using Godot;
using System;

namespace tee
{

	public delegate void TopicButtonHandler();
	public partial class TopicButton : Button
	{
		public static event TopicButtonHandler OnButtonPressed;
		private ConversationTopic _conversationTopic;
		private AttackButton _parentButton;
		public AttackButton ParentButton
		{
			get { return _parentButton; }
			set { _parentButton = value; }
		}
		public ConversationTopic ConversationTopic
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
			_parentButton.ChildPressed(this);
			OnButtonPressed?.Invoke();
		}
	}
}
