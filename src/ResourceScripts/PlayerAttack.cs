using Godot;
using System;

namespace tee
{
	public partial class PlayerAttack : Attack
	{
		private CharacterName _owningCharacter;
		private int _conversationInterestDamage;
		private bool _enableTopicChoice;
		private bool _isFromItem;
		private Godot.Collections.Array<TopicName> _unlockedTopics = new();
		[Export]
		private Godot.Collections.Dictionary<TopicName, string> _topicRelatedQuotes = new(){
			{TopicName.None, ""},
			{TopicName.Sport, ""},
			{TopicName.Lifestyle, ""},
			{TopicName.Economy, ""},
			{TopicName.Politics, ""},
			{TopicName.Art, ""},
			{TopicName.Gossip, ""},
			{TopicName.Food, ""},
			{TopicName.Drink, ""}
		};
		private BonusEffect _bonusEffect;
		[Export]
        public CharacterName OwningCharacter
        {
			get{return _owningCharacter;}
			set{_owningCharacter = value;}
		}
		[Export]
		/// <summary> Is always a positive value.</summary>
		public int ConversationInterestDamage
		{
			get { return _conversationInterestDamage; }
			set { _conversationInterestDamage = Math.Abs(value); }
		}
		[Export]
		public bool EnableTopicChoice
		{
			get { return _enableTopicChoice; }
			set { _enableTopicChoice = value; }
		}
		[Export]
		public bool IsFromItem
		{
			get { return _isFromItem; }
			set { _isFromItem = value; }
		}
		[Export]
		public Godot.Collections.Array<TopicName> UnlockedTopics
		{
			get { return _unlockedTopics; }
			set { _unlockedTopics = value; }
		}

		public string GetQuoteForTopic(TopicName topic){
			return _topicRelatedQuotes[topic];
		}

		
		/// <summary>
		/// Resolves the stats of this PlayerAttack and its BonusEffect, if it is not null.
		/// </summary>
		/// <param name="combatManager">
		///	The CombatManager to resolve this PlayerAttack on.
		/// </param>
		public void Resolve(CombatManager combatManager){
			combatManager.ConversationInterestDamage = _conversationInterestDamage;
			_bonusEffect?.Resolve(combatManager);
		}
	}
}
