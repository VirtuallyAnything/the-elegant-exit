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
		private Array<EnemyAttack> _attackPool;
		private int _conversationInterest;
		private AnnoyanceLevel _annoyance = new();
		private System.Collections.Generic.Dictionary<TopicName, TopicPreference> _topicPreferences;
		private List<TopicName> _likes = new();
		private List<TopicName> _neutrals = new();
		private List<TopicName> _dislikes = new();
		private TopicName _lastTopicName = TopicName.None;
		private TopicName _currentTopicName = TopicName.None;
		public string DisplayName
		{
			get { return _displayName; }
		}
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
		public bool IgnoreNextAnnoyance
		{
			get; set;
		}
		public bool IgnoreNextEnthusiasm
		{
			get; set;
		}

		public EncounterEnemy(EnemyData data)
		{
			_displayName = data.DisplayName;
			_enemyAttacks = data.EnemyAttacks;
			_attackPool = new(_enemyAttacks);
			_conversationInterest = data.ConversationInterest;

			_topicPreferences = new();
			foreach (TopicName topicName in data.Likes)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Like);
				_topicPreferences.Add(topicName, topicPreference);
				_likes.Add(topicName);
			}

			foreach (TopicName topicName in data.Neutrals)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Neutral);
				_topicPreferences.Add(topicName, topicPreference);
				_neutrals.Add(topicName);
			}

			foreach (TopicName topicName in data.Dislikes)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), Preference.Dislike);
				_topicPreferences.Add(topicName, topicPreference);
				_dislikes.Add(topicName);
			}
		}

		public override void _Ready()
		{
			_annoyance.ConversationInterestChanged += UpdateConversationInterest;
			EnthusiasmLevel.AnnoyanceLowered += DecreaseAnnoyance;
			_topicPreferences[TopicName.None].ConversationTopic.Weight = 0;
			_topicPreferences[TopicName.Weather].ConversationTopic.Weight = 0;
		}

		public TopicName ChooseTopic()
		{
			List<ConversationTopic> allTopics = [];
			foreach (TopicName topic in _likes)
			{
				allTopics.Add(_topicPreferences[topic].ConversationTopic);
			}
			foreach (TopicName topic in _neutrals)
			{
				allTopics.Add(_topicPreferences[topic].ConversationTopic);
			}

			ConversationTopic chosenTopic = allTopics.WeightedRandom<ConversationTopic>();
			GD.Print($"{DisplayName} chooses topic {chosenTopic.Name}.");
			return chosenTopic.Name;
		}

		public EnemyAttack ChooseAttack(TopicName topicName)
		{
			_currentTopicName = topicName;
			ConversationTopic chosenTopic = _topicPreferences[topicName].ConversationTopic;
			Array<EnemyAttack> potentialAttacks = new();
			foreach (EnemyAttack attack in _attackPool)
			{
				if (attack.Topic == _currentTopicName)
				{
					potentialAttacks.Add(attack);
				}
			}

			if (potentialAttacks.Count == 1)
			{
				chosenTopic.Weight = 0;
			}
			else
			{
				chosenTopic.Weight += 5;
			}

			if (potentialAttacks.Count == 0)
			{
				GD.Print($"No more attacks for topic {topicName} available. Choosing different topic.");
				return ChooseAttack(ChooseTopic());
			}
			else
			{
				EnemyAttack chosenAttack = potentialAttacks.PickRandom();
				GD.Print($"{DisplayName} has {potentialAttacks.Count} attacks for {chosenTopic.Name}. They choose {chosenAttack.AttackName}.");

				_attackPool.Remove(chosenAttack);
				return chosenAttack;
			}
		}

		public void ReactTo(TopicName topicName)
		{
			if (topicName == TopicName.None)
			{
				return;
			}

			_lastTopicName = _currentTopicName;
			_currentTopicName = topicName;

			if (topicName == TopicName.Weather)
			{
				return;
			}

			ConversationTopic currentConversationTopic = _topicPreferences[_currentTopicName].ConversationTopic;

			//if there is a change of topic to a topic with less than two enthusiasm, increase annoyance.
			if (_currentTopicName != _lastTopicName)
			{
				if (currentConversationTopic.GetCurrentEnthusiasmLevel() < 2 && !IgnoreNextAnnoyance)
				{
					ConversationTopic lastConversationTopic = _topicPreferences[_lastTopicName].ConversationTopic;
					int currentEnthusiasm = lastConversationTopic.GetCurrentEnthusiasmLevel();
					switch (currentEnthusiasm)
					{
						case 2:
						case 3:
							_annoyance.Increase();
							GD.Print($"{DisplayName} was annoyed to be changing the topic. Annoyance increases by one to {_annoyance.CurrentAnnoyance}.");
							break;
						case 4:
						case 5:
							_annoyance.Increase(2);
							GD.Print($"{DisplayName} was very annoyed to be changing the topic. Annoyance increases by two to {_annoyance.CurrentAnnoyance}.");
							break;
					}
				}
			}

			Preference preference = GetPreferenceFor(_currentTopicName);
			switch (preference)
			{
				case Preference.Like:
				case Preference.Neutral:
					if (!IgnoreNextEnthusiasm)
					{
						currentConversationTopic.IncreaseEnthusiasm();
					}
					break;
				case Preference.Dislike:
					if (!IgnoreNextAnnoyance)
					{
						_annoyance.Increase();
					}
					break;
			}
			GD.Print($"{DisplayName} Preference for {_currentTopicName}: {preference}");
			IgnoreNextAnnoyance = false;
			IgnoreNextEnthusiasm = false;
		}

		public Preference GetPreferenceFor(TopicName topic)
		{
			return _topicPreferences[topic].Preference;
		}

		public void Enrage(TopicName dislikedTopicName)
		{
			GD.Print($"{DisplayName} hates {dislikedTopicName}! Stop bringing it up!");
		}

		public void UpdateConversationInterest(int summand)
		{
			_conversationInterest += summand;
		}

		public void IncreaseEnthusiasmFor(TopicName topic)
		{
			_topicPreferences[topic].ConversationTopic.IncreaseEnthusiasm();
		}

		public void DecreaseEnthusiasmFor(TopicName topic)
		{
			_topicPreferences[topic].ConversationTopic.DecreaseEnthusiasm();
		}
		
		public void DecreaseAnnoyance()
		{
			_annoyance.Decrease();
		}

		public void IncreaseAnnoyance()
		{
			_annoyance.Increase();
		}
	}
}
