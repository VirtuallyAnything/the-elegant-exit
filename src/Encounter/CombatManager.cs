using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace tee
{
	public delegate void CombatEventHandler();
	public delegate void CombatEndHandler(EncounterOutcome outcome, int socialStanding, int socialBattery);
	public delegate void CombatOutcomeHandler();
	public delegate void PreferenceEventHandler(TopicName topicName, Preference preference);
	public partial class CombatManager : Node
	{
		public static event CombatEndHandler CombatEnded;
		public static event CombatEventHandler EnemyTurnComplete;
		public static event CombatEventHandler TopicalAttackActive;
		public static event PreferenceEventHandler PreferenceDiscovered;
		[Export] private EncounterScene _encounterScene;
		private EncounterPlayer _player;
		private PlayerAttack _selectedAttack;
		private EncounterEnemy _enemy;
		private Preference _preferenceForCurrentTopic;
		private TopicName _nextTopicName;
		private int _conversationInterestDamage;
		private int _conversationInterestBonusDamage;
		private bool _isBlockNextEnemyAttack;
		private bool _isIgnoreCIBonusDamage;
		[Export] private int _socialBatteryPenalty = 5;
		private static int _dynamicCIDamageLike = -1;
		private static int _dynamicCIDamageDislike = 1;
		public int ConversationInterestDamage
		{
			get { return _conversationInterestDamage; }
			set { _conversationInterestDamage = value; }
		}
		public int ConversationInterestBonusDamage
		{
			get { return _conversationInterestBonusDamage; }
			set { _conversationInterestBonusDamage = value; }
		}
		public Preference PreferenceForCurrentTopic
		{
			get { return _preferenceForCurrentTopic; }
			set { _preferenceForCurrentTopic = value; }
		}
		public EncounterEnemy Enemy
		{
			get { return _enemy; }
		}
		public EncounterPlayer Player
		{
			get { return _player; }
		}
		public PlayerAttack SelectedAttack
		{
			get { return _selectedAttack; }
		}
		public TopicName NextTopicName
		{
			get { return _nextTopicName; }
			set
			{
				if (_enemy.GetPreferenceFor(value) != Preference.Dislike && _enemy.GetPreferenceFor(value) != Preference.Unknown)
				{
					_nextTopicName = value;
				}
				else
				{
					_nextTopicName = TopicName.None;
				}
				;
			}
		}
		public int SocialStanding
		{
			get; set;
		}

		public override void _Ready()
		{
			_player = new(new List<PlayerAttack>(GameManager.AvailableAttacks));

			_encounterScene.SetupCompleted += StartCombat;
			EncounterScene.PlayerTurnComplete += OnPlayerTurnComplete;
			AttackCard.AttackSelected += PlayerAttack;
			AnnoyanceLevel.Changed += EndCombat;
		}

		public async void StartCombat()
		{
			PlayerAttack randomAttack;
			for (int i = 0; i <= 3; i++)
			{
				randomAttack = Player.ChooseRandomAttack();
				_encounterScene.AttackCardContainer.AddNewAttackCard(randomAttack);
			}
			_enemy = new(_encounterScene.CurrentEnemy);

			_encounterScene.AttackCardContainer.DisableInput();
			await _encounterScene.PlayCombatStartAnimation(Player.MaxMentalCapacity);

			GameManager.SocialBattery -= Player.MaxMentalCapacity;
			Player.MentalCapacity = Player.MaxMentalCapacity;
			await EnemyAttack();
			Player.UpdateCurrentAttacks(this);
			_encounterScene.AttackCardContainer.Update();
			EnemyTurnComplete += SetupNewAttack;
		}

		public void OnPlayerTurnComplete()
		{
			EnemyAttack();
		}

		/// <summary>
		/// Called when either the Player's MentalCapacity or the Enemy's Conversation Interest runs out
		/// </summary>
		/// <param name="hasPlayerWon">true, if the Enemy stats are depleted before the Player's.</param>
		private void EndCombat(EncounterOutcome outcome)
		{
			SocialStanding += Enemy.GetTotalSocialStanding();
			int socialBatteryPenalty = 0;
			if (outcome == EncounterOutcome.PlayerDefeated)
			{
				socialBatteryPenalty = _socialBatteryPenalty;
			}
			CombatEnded?.Invoke(outcome, SocialStanding, Player.MentalCapacity - socialBatteryPenalty);
		}

		private void EndCombat(AnnoyanceData data)
		{
			if (data.CurrentAnnoyance == AnnoyanceLevel.MaxAnnoyance)
			{
				CombatEnded?.Invoke(EncounterOutcome.MaxAnnoyanceReached, data.SocialStandingChange, Player.MentalCapacity);
			}
		}

		private void SetupNewAttack()
		{
			PlayerAttack newAttack = Player.SwapAttackOut(_selectedAttack);
			Player.UpdateCurrentAttacks(this);
			_encounterScene.AttackCardContainer.SwapAttackCardOutFor(newAttack);
		}

		public async void PlayerAttack(PlayerAttack playerAttack)
		{
			_selectedAttack = playerAttack;
			Player.LastTopicName = Player.CurrentTopicName;
			Player.CurrentTopicName = TopicName.None;
			PreferenceForCurrentTopic = Preference.Unknown;

			// Set the base attack damage
			ConversationInterestDamage = _selectedAttack.ConversationInterestDamage;
			ConversationInterestBonusDamage = 0;

			TopicName topicOfAttack;
			if (playerAttack is TopicalPlayerAttack topicalPlayerAttack)
			{
				TopicalAttackActive?.Invoke();
				topicOfAttack = topicalPlayerAttack.SelectedTopicName;
				Player.CurrentTopicName = topicOfAttack;
				PreferenceForCurrentTopic = Enemy.GetPreferenceFor(topicOfAttack);
				_selectedAttack.BonusEffect?.Resolve(this);
				if (!_isIgnoreCIBonusDamage)
				{
					switch (PreferenceForCurrentTopic)
					{
						// Apply damage modifiers for Preferences
						case Preference.Like:
							ConversationInterestBonusDamage += _dynamicCIDamageLike;
							break;
						case Preference.Dislike:
							ConversationInterestBonusDamage += _dynamicCIDamageDislike;
							if (Player.CurrentTopicName == Player.LastTopicName)
							{
								Enemy.Enrage(Player.CurrentTopicName);
							}
							// Subtract one social Battery if the player knowingly talks about a disliked topic
							if (Player.DiscoveredEnemyPreferences.ContainsKey(Player.CurrentTopicName))
							{
								GameManager.SocialBattery -= 1;
							}
							break;
					}
				}
			}
			else
			{
				topicOfAttack = TopicName.None;
				_selectedAttack.BonusEffect?.Resolve(this);
			}

			if (Player.CurrentTopicName == TopicName.Weather)
			{
				Enemy.ReactTo(Player.CurrentTopicName);
			}
			else
			{
				Enemy.ReactTo(topicOfAttack);
			}

			// Add newly discovered preference
			if (Player.CurrentTopicName != TopicName.None)
			{
				PreferenceDiscovered?.Invoke(Player.CurrentTopicName, _preferenceForCurrentTopic);
			}

			// Actually subtract the damages from Conversation Interest
			Enemy.ConversationInterest -= ConversationInterestBonusDamage + ConversationInterestDamage;
			GD.Print($"Player attacks with {playerAttack.AttackName} and does {ConversationInterestBonusDamage + ConversationInterestDamage} damage to CI.");
			GD.Print($"New Enemy CI: {Enemy.ConversationInterest}/{Enemy.ConversationInterestMax}");
			_isIgnoreCIBonusDamage = false;

			// Wait for all the animations to finish
			await _encounterScene.PlayDialogAnimation(_selectedAttack);
			await _encounterScene.PlayAnimationsForAttack(_selectedAttack, ConversationInterestBonusDamage);
			await _encounterScene.UpdateConversationInterestMax();

			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);

			if (Enemy.ConversationInterest <= 0)
			{
				EndCombat(EncounterOutcome.EnemyDefeated);
				return;
			}

			_encounterScene.EnableInput(); //DO I even need this
		}

		public async Task EnemyAttack()
		{
			TopicName chosenTopicName;
			//Check if next topic was already determined by player
			if (NextTopicName != TopicName.None)
			{
				chosenTopicName = NextTopicName;
				NextTopicName = TopicName.None;
			}
			else
			{
				chosenTopicName = Enemy.ChooseTopic();
			}

			EnemyAttack enemyAttack = Enemy.ChooseAttack(chosenTopicName);
			await _encounterScene.PlayDialogAnimation(enemyAttack);

			if (_isBlockNextEnemyAttack)
			{
				_isBlockNextEnemyAttack = false;
			}
			else
			{
				GameManager.SocialBattery += enemyAttack.SocialBatteryChange;
				Player.MentalCapacity -= enemyAttack.MentalCapacityDamage;
				await _encounterScene.PlayAnimationsForAttack(enemyAttack);
			}

			Enemy.IncreaseEnthusiasmFor(chosenTopicName);
			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);
			await _encounterScene.UpdateConversationInterestMax();

			if (Player.MentalCapacity <= 0)
			{
				EndCombat(EncounterOutcome.PlayerDefeated);
				return;
			}

			EnemyTurnComplete?.Invoke();
		}

		public void BlockNextEnemyAttack()
		{
			_isBlockNextEnemyAttack = true;
		}

		public void IgnoreCIBonusDamage()
		{
			_isIgnoreCIBonusDamage = true;
		}

		public void IgnoreNextAnnoyance()
		{
			Enemy.IsIgnoreNextAnnoyance = true;
		}

		public void IgnoreTopicSwitchAnnoyance()
		{
			Enemy.IsIgnoreTopicSwitchAnnoyance = true;
		}

		public void IgnoreNextEnthusiasm()
		{
			Enemy.IsIgnoreNextEnthusiasm = true;
		}

		public static int GetDynamicCIDamageFor(Preference preference)
		{
			switch (preference)
			{
				case Preference.Like:
					return _dynamicCIDamageLike;
				case Preference.Dislike:
					return _dynamicCIDamageDislike;
			}
			return 0;
		}

		public override void _ExitTree()
		{
			_encounterScene.SetupCompleted -= StartCombat;
			EncounterScene.PlayerTurnComplete -= OnPlayerTurnComplete;
			EnemyTurnComplete -= SetupNewAttack;
			AttackCard.AttackSelected -= PlayerAttack;
			AnnoyanceLevel.Changed -= EndCombat;
			_enemy.Free();
		}
	}
}
