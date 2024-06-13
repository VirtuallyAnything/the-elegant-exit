using Godot;
using System;

namespace tee
{

	public delegate void TopicButtonHandler(ConversationTopic conversationTopic);
	public partial class TopicButton : Button
	{
		public static event TopicButtonHandler OnButtonPressed;
		private ConversationTopic _conversationTopic;
		private ButtonGrid _parentGrid;
		public ButtonGrid ParentGrid
		{
			get { return _parentGrid; }
			set { _parentGrid = value; }
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
			_parentGrid.DisableAllButtons();
			OnButtonPressed?.Invoke(_conversationTopic);
		}

		private void Disable(ConversationTopic topic)
		{
			Disabled = true;
		}
	}
}
