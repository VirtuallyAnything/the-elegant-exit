using Godot;
using System;
using Godot.Collections;

namespace tee
{
	public delegate void AttackButtonHandler(AttackButton attackButton);
	public partial class AttackButton : Button
	{
		private PlayerAttack _boundAttack;
		private TopicName _boundTopic;
		private ButtonGrid _topicGrid;
		[Export] AttackValueSetter _attackValueSetter;
		private Array<TopicButton> _topicButtonsAttack = new();
		private Array<TopicName> _attackConversationTopics = new();
		private Array<TopicButton> _topicButtonsItem = new();
		private Array<TopicName> _itemConversationTopics = new();
		public static event AttackButtonHandler OnButtonPressed;
		[Export]
		public ButtonGrid TopicGrid
		{
			get { return _topicGrid; }
			set { _topicGrid = value; }
		}
		public PlayerAttack BoundAttack
		{
			get { return _boundAttack; }
			set { _boundAttack = value; }
		}
		public TopicName BoundTopic
		{
			get { return _boundTopic; }
			set { _boundTopic = value; }
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			TopicButton.OnButtonPressed += Disable;
			EncounterScene.EnemyTurnAnimationComplete += Enable;
			_attackConversationTopics.Add(TopicName.Art);
			_attackConversationTopics.Add(TopicName.Economy);
			_attackConversationTopics.Add(TopicName.Gossip);
			_attackConversationTopics.Add(TopicName.Politics);
			_attackConversationTopics.Add(TopicName.Sport);
			_attackConversationTopics.Add(TopicName.Lifestyle);

			foreach (TopicName conversationTopic in _attackConversationTopics)
			{
				TopicButton button = new()
				{
					Text = conversationTopic.ToString(),
					ConversationTopic = conversationTopic
				};
				_topicButtonsAttack.Add(button);
			}

			_itemConversationTopics.Add(TopicName.Drink);
			_itemConversationTopics.Add(TopicName.Food);

			foreach (TopicName conversationTopic in _itemConversationTopics)
			{
				TopicButton button = new()
				{
					Text = conversationTopic.ToString(),
					ConversationTopic = conversationTopic
				};
				_topicButtonsItem.Add(button);
			}
		}

		public void ChildPressed(TopicButton button){
			_topicGrid.DisableAllButtons();
			_boundTopic = button.ConversationTopic;
			OnButtonPressed?.Invoke(this);
		}

		public void SetupButton(PlayerAttack attack)
		{
			foreach (Button child in _topicGrid.ChildButtons)
			{
				_topicGrid.RemoveChild(child);
			}
			_topicGrid.ChildButtons.Clear();
			_boundAttack = attack;
			Text = attack.AttackName;
			if (attack.EnableTopicChoice)
			{
				if (attack.IsFromItem)
				{
					foreach (TopicButton button in _topicButtonsItem)
					{
						_topicGrid.Add(button);
						button.ParentButton = this;
						if (!attack.UnlockedTopics.Contains(button.ConversationTopic))
						{
							button.Disabled = true;
						}
						else
						{
							button.Disabled = false;
						}
					}
				}
				else
				{
					foreach (TopicButton button in _topicButtonsAttack)
					{
						_topicGrid.Add(button);
						button.ParentButton = this;
						if (!attack.UnlockedTopics.Contains(button.ConversationTopic))
						{
							button.Disabled = true;
						}
						else
						{
							button.Disabled = false;
						}
					}
				}
			}
			else
			{
				TopicButton button = new()
				{
					Text = "Select",
					ConversationTopic = TopicName.None
				};
				_topicGrid.Add(button);
				button.ParentButton = this;
			}
			_attackValueSetter.SetLabels(attack);
		}

		private void Disable()
		{
			Disabled = true;
		}

		private void Enable()
		{
			Disabled = false;
		}

		public override void _ExitTree()
		{
			TopicButton.OnButtonPressed -= Disable;
			EncounterScene.EnemyTurnAnimationComplete -= Enable;
		}
    }
}
