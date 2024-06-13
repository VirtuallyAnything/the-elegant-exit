using Godot;
using System;
[Tool]
public partial class PlayerAttack : Attack
{
	private float _socialStandingChangeLike,
		_socialStandingChangeDislike;  
	private float _interestChange;
	private bool _enableTopicChoice;
	private bool _isFromItem;
	private Godot.Collections.Array<ConversationTopic> _unlockedTopics = new();

    [Export] public float SocialStandingChangeLike 
	{ 
		get{return _socialStandingChangeLike;}
		set{_socialStandingChangeLike = value;} 
	}
	[Export] public float SocialStandingChangeDislike 
	{ 
		get{return _socialStandingChangeDislike;}
		set{_socialStandingChangeDislike = value;} 
	}
	[Export] public float InterestChange 
	{ 
		get{return _interestChange;}
		set{_interestChange = value;} 
	}
	[Export] public bool EnableTopicChoice{
		get{return _enableTopicChoice;}
		set{_enableTopicChoice = value;}
	}
	[Export] public bool IsFromItem{
		get{return _isFromItem;}
		set{_isFromItem = value;}
	}
	[Export] public Godot.Collections.Array<ConversationTopic> UnlockedTopics{
		get{return _unlockedTopics;}
		set{_unlockedTopics = value;}
	}
}
