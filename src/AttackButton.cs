using Godot;
using System;
using Godot.Collections;

public delegate void AttackButtonHandler(AttackButton attackButton);
public partial class AttackButton : Button
{
	private PlayerAttack _boundAttack;
	private ButtonGrid _topicGrid;
	private Array<TopicButton> _topicButtonsAttack = new();
	private Array<ConversationTopic> _attackConversationTopics = new();
	private Array<TopicButton> _topicButtonsItem = new();
	private Array<ConversationTopic> _itemConversationTopics = new();
	public static event AttackButtonHandler OnButtonPressed;
	[Export]public ButtonGrid TopicGrid{
		get{return _topicGrid;}
		set{_topicGrid = value;}
	}
	public PlayerAttack BoundAttack{
		get{return _boundAttack;}
		set{_boundAttack = value;}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnPressed;
		TopicButton.OnButtonPressed += Disable;
		EncounterScene.EnemyTurnAnimationComplete += Enable;
		_attackConversationTopics.Add(ConversationTopic.Art);
		_attackConversationTopics.Add(ConversationTopic.Economy);
		_attackConversationTopics.Add(ConversationTopic.Gossip);
		_attackConversationTopics.Add(ConversationTopic.Politics);
		_attackConversationTopics.Add(ConversationTopic.Sport);
		_attackConversationTopics.Add(ConversationTopic.Weather);

		foreach(ConversationTopic conversationTopic in _attackConversationTopics){
            TopicButton button = new()
            {
                Text = conversationTopic.ToString(),
                ConversationTopic = conversationTopic
            };
            _topicButtonsAttack.Add(button);
		}

		_itemConversationTopics.Add(ConversationTopic.PartyGossip);
		_itemConversationTopics.Add(ConversationTopic.Drink);
		_itemConversationTopics.Add(ConversationTopic.Food);

		foreach(ConversationTopic conversationTopic in _itemConversationTopics){
            TopicButton button = new()
            {
                Text = conversationTopic.ToString(),
                ConversationTopic = conversationTopic
            };
            _topicButtonsItem.Add(button);
		}
	}

	public void SetupButton(PlayerAttack attack){
		foreach(Button child in _topicGrid.ChildButtons){
			_topicGrid.RemoveChild(child);
		}
		_boundAttack = attack;
		Text = attack.AttackName;
		if(attack.EnableTopicChoice){
			if(attack.IsFromItem){
				foreach(TopicButton button in _topicButtonsItem){
					_topicGrid.AddChild(button);
					if(!attack.UnlockedTopics.Contains(button.ConversationTopic)){
						button.Disabled = true;
					}else{
						button.Disabled = false;
					}
				}
			}else{
				foreach(TopicButton button in _topicButtonsAttack){
					_topicGrid.Add(button);
					if(!attack.UnlockedTopics.Contains(button.ConversationTopic)){
						button.Disabled = true;
					}else{
						button.Disabled = false;
					}
				}
			}
			
		}else{
			TopicButton button = new(){
				Text = "Select",
				ConversationTopic = ConversationTopic.None
			};
			_topicGrid.Add(button);
		}	
	}

	private void Disable(ConversationTopic topic){
		Disabled = true;
	}

	private void Enable(){
		Disabled = false;
	}

	public void OnPressed(){
        OnButtonPressed?.Invoke(this);
    }

    public override void _ExitTree()
    {
        TopicButton.OnButtonPressed -= Disable;
		EncounterScene.EnemyTurnAnimationComplete -= Enable;
    }
}
