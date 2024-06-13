using Godot;
using System;

public delegate void TopicButtonHandler(ConversationTopic conversationTopic);
public partial class TopicButton : Button
{
	public static event TopicButtonHandler OnButtonPressed;
	private ConversationTopic _conversationTopic;
	public ConversationTopic ConversationTopic{
		get{return _conversationTopic;}
		set{_conversationTopic = value;}
	}

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    public void OnPressed(){
		OnButtonPressed?.Invoke(_conversationTopic);
	}

	private void Disable(ConversationTopic topic){
		Disabled = true;
	}

    public override void _ExitTree()
    {
    }

}
