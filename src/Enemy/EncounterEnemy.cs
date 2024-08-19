using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace tee
{
	struct TopicPreference
	{
		public ConversationTopic ConversationTopic
		{
			get;
		}
		public Preference Preference
		{
			get;
		}

		public TopicPreference(ConversationTopic conversationTopic, Preference preference)
		{
			ConversationTopic = conversationTopic;
			Preference = preference;
		}
	}
	public partial class EncounterEnemy : Node
	{
		private string _displayName;
		private Array<EnemyAttack> _enemyAttacks;
		private int _conversationInterest;
		private AnnoyanceLevel _annoyance = new();
		private System.Collections.Generic.Dictionary<TopicName, TopicPreference> _topicPreferences;
		private List<TopicName> _likes = new();
		private List<TopicName> _neutrals = new();
		private List<TopicName> _dislikes = new();
		private TopicName _lastTopicName = TopicName.None;
		private TopicName _currentTopicName = TopicName.None;
		public int ConversationInterest
		{
			get { return _conversationInterest; }
			set
			{
				if (value >= 0)
				{
					_conversationInterest = value;
				}
				else
				{
					_conversationInterest = 0;
				}
			}
		}
		public AnnoyanceLevel Annoyance
		{
			get { return _annoyance; }
		}
		public List<TopicName> Likes
		{
			get { return _likes; }
		}
		public List<TopicName> Neutrals
		{
			get { return _neutrals; }
		}
		public List<TopicName> Dislikes
		{
			get { return _dislikes; }
		}

		public EncounterEnemy(EnemyData data)
		{
			_displayName = data.DisplayName;
			_enemyAttacks = data.EnemyAttacks;
			_conversationInterest = data.ConversationInterest;

			_topicPreferences = new();
			foreach (TopicName topicName in data.Likes)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Like);
				_topicPreferences.Add(topicName, topicPreference);
			}

			foreach (TopicName topicName in data.Neutrals)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Neutral);
				_topicPreferences.Add(topicName, topicPreference);
			}

			foreach (TopicName topicName in data.Dislikes)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Dislike);
				_topicPreferences.Add(topicName, topicPreference);
			}
		}

		public override void _Ready()
		{
			_annoyance.ConversationInterestChanged += UpdateConversationInterest;
			EnthusiasmLevel.AnnoyanceLowered += DecreaseAnnoyance;
		}

		public ConversationTopic ChooseTopic()
		{
			List<ConversationTopic> allTopics = [];
			foreach(TopicName topic in _likes){
				allTopics.Add(_topicPreferences[topic].ConversationTopic);
			}
			foreach(TopicName topic in _neutrals){
				allTopics.Add(_topicPreferences[topic].ConversationTopic);
			}
			return allTopics.WeightedRandom<ConversationTopic>();
		}

		public EnemyAttack ChooseAttack()
		{
			TopicName topic = ChooseTopic().Name;
			Array<EnemyAttack> potentialAttacks = new();
			foreach (EnemyAttack attack in _enemyAttacks)
			{
				if (attack.Topic == topic)
				{
					potentialAttacks.Add(attack);
				}
			}
			EnemyAttack chosenAttack = potentialAttacks.PickRandom();
			_currentTopicName = chosenAttack.Topic;
			return chosenAttack;
		}

		public Preference GetPreferenceFor(TopicName topic)
		{
			return _topicPreferences[topic].Preference;
		}

		public void IncreaseEnthusiasmFor(TopicName topic){
			_topicPreferences[topic].ConversationTopic.IncreaseEnthusiasm();
		}

		public void DecreaseEnthusiasmFor(TopicName topic){
			_topicPreferences[topic].ConversationTopic.DecreaseEnthusiasm();
		}

		public void ReactTo(TopicName topic)
		{
			_lastTopicName = _currentTopicName;
			_currentTopicName = topic;
			ConversationTopic currentConversationTopic = _topicPreferences[_currentTopicName].ConversationTopic;
			Preference preference = GetPreferenceFor(_currentTopicName);
			switch (preference)
			{
				case Preference.Like:
					currentConversationTopic.IncreaseEnthusiasm();
					break;
				case Preference.Neutral:
					currentConversationTopic.IncreaseEnthusiasm();
					break;
				case Preference.Dislike:
					_annoyance.Increase();
					if (_currentTopicName == _lastTopicName)
					{
						//Enrage Dialogue
					}
					break;
			}

			if (_lastTopicName != _currentTopicName)
			{
				if (currentConversationTopic.GetCurrentEnthusiasmLevel() < 2)
				{
					ConversationTopic lastConversationTopic = _topicPreferences[_lastTopicName].ConversationTopic;
					int currentEnthusiasm = lastConversationTopic.GetCurrentEnthusiasmLevel();
					switch (currentEnthusiasm)
					{
						case 2:
						case 3:
							_annoyance.Increase();
							break;
						case 4:
						case 5:
							_annoyance.Increase(2);
							break;
					}
				}
			}
		}

		public void UpdateConversationInterest(int summand)
		{
			_conversationInterest += summand;
		}

		public void DecreaseAnnoyance(){
			_annoyance.Decrease();
		}

		public void IncreaseAnnoyance(){
			_annoyance.Increase();
		}
	}
}
