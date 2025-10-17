using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Godot;
using Godot.Collections;

namespace tee
{
	readonly struct TopicPreference
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
	public partial class EncounterEnemy : EncounterCharacter
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
		private List<TopicName> _likes;
		private List<TopicName> _neutrals;
		private List<TopicName> _dislikes;
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
		public bool IsIgnoreNextAnnoyance
		{
			get; set;
		}
		public bool IsIgnoreTopicSwitchAnnoyance
		{
			get; set;
		}
		public bool IsIgnoreNextEnthusiasm
		{
			get; set;
		}

		public EncounterEnemy(EnemyData data)
		{
			_displayName = data.DisplayName;
			_enemyAttacks = StaticData.LoadJsonFile(data.PathToAttacks);
			_attackPool = new(_enemyAttacks);
			ConversationInterest = data.ConversationInterest;
			ConversationInterestMax = _conversationInterest;

			_topicPreferences = new();
			_likes = SetupPreferences(data.Likes, Preference.Like);
			_neutrals = SetupPreferences(data.Neutrals, Preference.Neutral);
			_dislikes = SetupPreferences(data.Dislikes, Preference.Dislike);

			AnnoyanceLevel.Changed += UpdateConversationInterest;
			ConversationTopic.EnthusiasmChangedForTopic += UpdateConversationInterest;
			EnthusiasmLevel.AnnoyanceLowered += DecreaseAnnoyance;
		}

		private List<TopicName> SetupPreferences(Array<TopicName> dataArray, Preference preference)
		{
			List<TopicName> result = new();
			foreach (TopicName topicName in dataArray)
			{
				TopicPreference topicPreference = new(new ConversationTopic(topicName), preference);
				_topicPreferences.Add(topicName, topicPreference);
				result.Add(topicName);
			}
			return result;
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
				allTopics.Remove(_topicPreferences[CurrentTopicName].ConversationTopic);
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
			if (potentialAttacks.Count == 0)
			{
				GD.Print($"No more attacks for topic {topicName} available. Choosing different topic.");
				return ChooseAttack(ChooseTopic());
			}
			// Update last and current topic and their weights
			_lastTopicName = CurrentTopicName;
			CurrentTopicName = topicName;

			ConversationTopic chosenTopic = _topicPreferences[topicName].ConversationTopic;

			// With Count of 1, the list for this topic will be empty after this call.
			// Set the weight for chosenTopic to zero, so that it won't be chosen again.
			if (potentialAttacks.Count == 1)
			{
				chosenTopic.Weight = 0;
			}
			else
			{
				if (CurrentTopicName != _lastTopicName)
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

			_lastTopicName = CurrentTopicName;
			CurrentTopicName = topicName;

			if (topicName == TopicName.Weather)
			{
				_topicPreferences[_lastTopicName].ConversationTopic.Weight -= 5;
				return;
			}

			ConversationTopic currentConversationTopic = _topicPreferences[CurrentTopicName].ConversationTopic;
			// Current Topic gets +5 Weight, last Topic loses the extra Weight.
			// If they're the same topic, the Weight doesn't need to be changed.
			if (CurrentTopicName != _lastTopicName)
			{
				_topicPreferences[_lastTopicName].ConversationTopic.Weight -= 5;
				currentConversationTopic.Weight += 5;
			}

			//if there is a change of topic to a topic with less than two enthusiasm, increase annoyance.
			if (CurrentTopicName != _lastTopicName && !IsIgnoreTopicSwitchAnnoyance)
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
							GD.Print($"{DisplayName} was annoyed to be changing the topic from something they were enthusiastic about. Annoyance increases by one to {_annoyance.CurrentAnnoyance}.");
							break;
						case 4:
						case 5:
							_annoyance.Increase(2);
							GD.Print($"{DisplayName} was VERY annoyed to be changing the topic from something they were enthusiastic about. Annoyance increases by two to {_annoyance.CurrentAnnoyance}.");
							break;
					}
				}
			}

			// Increase Enthusiasm for the topic if the topic is liked or neutral in preference, 
			// otherwise increase annoyance
			Preference preference = GetPreferenceFor(CurrentTopicName);
			switch (preference)
			{
				// Missing content: Reaction dialogue based on preference for topic
				case Preference.Like:
				case Preference.Neutral:
					if (!IsIgnoreNextEnthusiasm)
					{
						currentConversationTopic.IncreaseEnthusiasm();
					}
					break;
				case Preference.Dislike:
					if (!IsIgnoreNextAnnoyance)
					{
						_annoyance.Increase();
					}
					break;
			}
			GD.Print($"{DisplayName} Preference for {CurrentTopicName}: {preference}");

			// Reset all one-time effect booleans
			IsIgnoreNextAnnoyance = false;
			IsIgnoreTopicSwitchAnnoyance = false;
			IsIgnoreNextEnthusiasm = false;
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

		public void UpdateConversationInterest(EnthusiasmData data, TopicName topicName)
		{
			ConversationInterest += data.ConversationInterestModDelta;
			ConversationInterestMax += data.ConversationInterestModDelta;
			_conversationInterestModifierEnthusiasm += data.ConversationInterestModDelta;
		}

		public void UpdateConversationInterest(AnnoyanceData data)
		{
			ConversationInterest += data.ConversationInterestModifier;
			ConversationInterestMax += data.ConversationInterestModifier;
			_conversationInterestModifierAnnoyance += data.ConversationInterestModifier;
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
			if (_topicPreferences.Keys.Contains(topic))
			{
				_topicPreferences[topic].ConversationTopic.DecreaseEnthusiasm();
			}
		}

		public void DecreaseAnnoyance()
		{
			_annoyance.Decrease();
		}

		public void IncreaseAnnoyance()
		{
			_annoyance.Increase();
		}

		public int GetTotalSocialStanding()
		{
			GD.Print("Retrieving Social Standing values determined by Enthusiasm and Annoyance:");
			int result = 0;
			IEnumerable<TopicName> likesNeutrals = _likes.Concat(_neutrals);
			foreach (TopicName topicName in likesNeutrals)
			{
				int value = _topicPreferences[topicName].ConversationTopic.GetCurrentSocialStanding();
				GD.Print($"Social Standing Bonus for {topicName}: {value}");
				result += value;
			}
			GD.Print($"Social Standing Penalty for Annoyance (Level {_annoyance.CurrentAnnoyance}): {Annoyance.SocialStandingChange}");
			return result + Annoyance.SocialStandingChange;
		}

		public override void _Notification(int what)
		{
			if (what == NotificationPredelete)
			{
				AnnoyanceLevel.Changed -= UpdateConversationInterest;
				ConversationTopic.EnthusiasmChangedForTopic -= UpdateConversationInterest;
				EnthusiasmLevel.AnnoyanceLowered -= DecreaseAnnoyance;
			}
		}
	}
}
