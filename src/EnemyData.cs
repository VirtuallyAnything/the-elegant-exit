using Godot;
using System;

public partial class EnemyData : Resource
{
	private string _displayName;
	private Godot.Collections.Array<EnemyAttack> _enemyAttacks;
	private Godot.Collections.Array<ConversationTopic> _topicPreferences;
	private Texture2D _icon;
	private Texture2D _sprite;
	private int _conversationInterest = 20;
	[Export] public string DisplayName{
		get{return _displayName;}
		set{_displayName = value;}
	}
	[Export] public Godot.Collections.Array<EnemyAttack> EnemyAttacks{
		get{return _enemyAttacks;}
		set{_enemyAttacks = value;}
	}
	[Export] public Texture2D Icon{
		get{return _icon;}
		set{_icon = value;}
	}
	[Export] public Texture2D Sprite{
		get{return _sprite;}
		set{_sprite = value;}
	}
	[Export] public int ConversationInterest{
		get{return _conversationInterest;}
		set{_conversationInterest = value;}
	}
	[Export] public Godot.Collections.Array<ConversationTopic> TopicPreferences{
		get{return _topicPreferences;}
		set{_topicPreferences = value;}
	}
}
