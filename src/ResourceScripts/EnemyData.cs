using Godot;
using System;

namespace tee
{
	public partial class EnemyData : Resource
	{
		private string _displayName;
		private Godot.Collections.Array<EnemyAttack> _enemyAttacks;
		private Godot.Collections.Array<TopicName> _likes = new([TopicName.SpecialInterest]);
		private Godot.Collections.Array<TopicName> _neutrals = new([TopicName.Art, TopicName.Economy, TopicName.Gossip, TopicName.Lifestyle, TopicName.Politics, TopicName.Sport]);
		private Godot.Collections.Array<TopicName> _dislikes;

		private Texture2D _icon;
		private Texture2D _sprite;
		private int _conversationInterest = 20;
		[Export]
		public string DisplayName
		{
			get { return _displayName; }
			set { _displayName = value; }
		}
		[Export]
		public Godot.Collections.Array<EnemyAttack> EnemyAttacks
		{
			get { return _enemyAttacks; }
			set { _enemyAttacks = value; }
		}
		[Export]
		public Texture2D Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}
		[Export]
		public Texture2D Texture
		{
			get { return _sprite; }
			set { _sprite = value; }
		}
		[Export]
		public int ConversationInterest
		{
			get { return _conversationInterest; }
			set { _conversationInterest = value; }
		}
		[Export]
		public Godot.Collections.Array<TopicName> Likes
		{
			get { return _likes; }
			set { _likes = value; }
		}
		[Export]
		public Godot.Collections.Array<TopicName> Neutrals
		{
			get { return _neutrals; }
			set { _neutrals = value; }
		}
		[Export]
		public Godot.Collections.Array<TopicName> Dislikes
		{
			get { return _dislikes; }
			set { _dislikes = value; }
		}
	}
}
