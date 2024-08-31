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
	public delegate void EncounterEnemyHandler(int value);
	public partial class EncounterEnemy : Node
	{
		public static event EncounterEnemyHandler ConversationInterestChanged;
		private string _displayName;
		private Array<EnemyAttack> _enemyAttacks;
		private Array<EnemyAttack> _attackPool;
		private int _conversationInterest;
		private int _conversationInterestMax;
		private int _conversationInterestModifierAnnoyance;
		private int _conversationInterestModifierEnthusiasm;
		private AnnoyanceLevel _annoyance = new();
		private System.Collections.Generic.Dictionary<TopicName, TopicPreference> _topicPreferences;
		private List<TopicName> _likes = new();
		private List<TopicName> _neutrals = new();
		private List<TopicName> _dislikes = new();
		private TopicName _lastTopicName = TopicName.None;
		private TopicName _currentTopicName = TopicName.None;
		private bool _switchTopic = false;
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
				ConversationInterestChanged?.Invoke(_conversationInterest);
			}
		}
		public int ConversationInterestMax
		{
			get { return _conversationInterestMax; }
			set
			{
				if (value >= 0)
				{
					_conversationInterestMax = value;
				}
				else
				{
					_conversationInterestMax = 0;
				}
				ConversationInterestChanged?.Invoke(_conversationInterestMax);
			}
		}
		public int ConversationInterestModifierAnnoyance
		{
			get { return _conversationInterestModifierAnnoyance; }
		}
		public int ConversationInterestModifierEnthusiasm
		{
			get { return _conversationInterestModifierEnthusiasm; }
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
		public TopicName CurrentTopicName
		{
			get { return _currentTopicName; }
		}
		public TopicName LastTopicName
		{
			get { return _lastTopicName; }
		}

		public EncounterEnemy(EnemyData data)
		{
			_displayName = data.DisplayName;
			_enemyAttacks = StaticData.GetLines(_displayName);
			_attackPool = new(_enemyAttacks);
			ConversationInterest = data.ConversationInterest;
			ConversationInterestMax = _conversationInterest;

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

			AnnoyanceLevel.ConversationInterestChanged += UpdateConversationInterest;
			EnthusiasmLevel.ConversationInterestChanged += UpdateConversationInterest;
			EnthusiasmLevel.AnnoyanceLowered += DecreaseAnnoyance;
		}

		public void SwitchTopicNextTurn()
		{
			_switchTopic = true;
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
			if (_switchTopic)
			{
				allTopics.Remove(_topicPreferences[_currentTopicName].ConversationTopic);
				_switchTopic = false;
			}

			ConversationTopic chosenTopic = allTopics.WeightedRandom<ConversationTopic>();
			GD.Print($"{DisplayName} chooses topic {chosenTopic.Name}.");
			return chosenTopic.Name;
		}

		public EnemyAttack ChooseAttack(TopicName topicName)
		{
			// Out of all enemy attacks that are still in the attack pool, list those that have the correct topic
			Array<EnemyAttack> potentialAttacks = new();
			foreach (EnemyAttack attack in _attackPool)
			{
				if (attack.Topic == topicName)
				{
					potentialAttacks.Add(attack);
				}
			}
			// If the list is empty, there must be no more attacks for this topic in the pool.
			// Call function recursively with different topic???
			if (potentialAttacks.Count == 0)
			{
				GD.Print($"No more attacks for topic {topicName} available. Choosing different topic.");
				return ChooseAttack(ChooseTopic());
			}
			// Update last and current topic and their weights
			_lastTopicName = _currentTopicName;
			_currentTopicName = topicName;

			ConversationTopic chosenTopic = _topicPreferences[topicName].ConversationTopic;

			// With Count of 1, the list for this topic will be empty after this call.
			// Set the weight for chosenTopic to zero, so that it won't be chosen again.
			if (potentialAttacks.Count == 1)
			{
				chosenTopic.Weight = 0;
			}
			else
			{
				if (_currentTopicName != _lastTopicName)
				{
					chosenTopic.Weight += 5;
					if (_lastTopicName != TopicName.None && _lastTopicName != TopicName.Weather)
					{
						_topicPreferences[_lastTopicName].ConversationTopic.Weight -= 5;
					}
				}
			}
			EnemyAttack chosenAttack = potentialAttacks.PickRandom();
			GD.Print($"{DisplayName} has {potentialAttacks.Count} attacks for {chosenTopic.Name}. They choose {chosenAttack.AttackName}.");

			// Every attack is only used once and then removed from the attack pool
			_attackPool.Remove(chosenAttack);
			return chosenAttack;
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
			// Current Topic gets +5 Weight, last Topic loses the extra Weight.
			// If they're the same topic, the Weight doesn't need to be changed.
			if (_currentTopicName != _lastTopicName)
			{
				_topicPreferences[_lastTopicName].ConversationTopic.Weight -= 5;
				currentConversationTopic.Weight += 5;
			}

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
			if (_topicPreferences.Keys.Contains(topic))
			{
				return _topicPreferences[topic].Preference;
			}
			else
			{
				return Preference.Unknown;
			}

		}

		public void Enrage(TopicName dislikedTopicName)
		{
			GD.Print($"{DisplayName} hates {dislikedTopicName}! Stop bringing it up!");
		}

		public void UpdateConversationInterest(int summand, GodotObject changedBy)
		{
			ConversationInterest += summand;
			ConversationInterestMax += summand;
			if (changedBy is EnthusiasmLevel)
			{
				_conversationInterestModifierEnthusiasm = 0;
				IEnumerable<TopicName> combined = Likes.Concat(Neutrals);
				foreach (TopicName topic in combined)
				{
					_conversationInterestModifierEnthusiasm += _topicPreferences[topic].ConversationTopic.GetConversationInterestModifier();
				}
			}
			else if (changedBy is AnnoyanceLevel)
			{
				_conversationInterestModifierAnnoyance += summand;
			}
		}

		public void IncreaseEnthusiasmFor(TopicName topic)
		{
			if (_dislikes.Contains(topic))
			{
				return;
			}
			_topicPreferences[topic].ConversationTopic.IncreaseEnthusiasm();
		}

		public void DecreaseEnthusiasmFor(TopicName topic)
		{
			_topicPreferences[topic].ConversationTopic.DecreaseEnthusiasm();
		}

		public int GetEnthusiasmLevelFor(TopicName topic)
		{
			if (_topicPreferences.Keys.Contains(topic))
			{
				return _topicPreferences[topic].ConversationTopic.GetCurrentEnthusiasmLevel();
			}
			return 0;

		}

		public void DecreaseAnnoyance()
		{
			_annoyance.Decrease();
		}

		public void IncreaseAnnoyance()
		{
			_annoyance.Increase();
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			AnnoyanceLevel.ConversationInterestChanged -= UpdateConversationInterest;
			EnthusiasmLevel.ConversationInterestChanged -= UpdateConversationInterest;
			EnthusiasmLevel.AnnoyanceLowered -= DecreaseAnnoyance;
		}
	}
}
