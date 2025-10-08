using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void CombatEventHandler();
	public delegate void CombatEndHandler(bool value);
	public delegate void CombatOutcomeHandler(int socialStanding, int socialBattery);
	public delegate void PreferenceEventHandler(TopicName topicName, Preference preference);
	public partial class CombatManager : Node
	{
		public static event CombatEndHandler CombatWon;
		public static event CombatOutcomeHandler FinalValuesDecided;
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
		private int _transferAmount = 10;
		private bool _isFirstTurn = true;
		private bool _isBlockNextEnemyAttack;
		private bool _isIgnoreCIBonusDamage;
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
			_player = new(GameManager.AvailableAttacks);

			_encounterScene.SetupCompleted += StartCombat;
			EncounterScene.PlayerTurnComplete += EnemyAttack;
			EnemyTurnComplete += SetupNewAttack;
			AttackCard.AttackSelected += PlayerAttack;
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
			await _encounterScene.PlayCombatStartAnimation(_transferAmount);

			GameManager.SocialBattery -= _transferAmount;
			Player.MentalCapacity = _transferAmount;
			EnemyAttack();
		}

		private void SetupNewAttack()
		{
			PlayerAttack newAttack = Player.SwapAttackOut(_selectedAttack);
			_encounterScene.AttackCardContainer.SwapAttackCardOutFor(newAttack);
		}

		public async void PlayerAttack(PlayerAttack playerAttack)
		{
			_isFirstTurn = false;

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
				if (!_isIgnoreCIBonusDamage)
				{
					switch (PreferenceForCurrentTopic)
					{
						// Apply damage modifiers for Preferences
						case Preference.Like:   //TODO: Give Player feedback for why they get -1 Damage to their attack
							ConversationInterestBonusDamage -= 1;
							break;
						case Preference.Dislike:
							ConversationInterestBonusDamage += 1;
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
			}

			_selectedAttack.BonusEffect?.Resolve(this);

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
				SocialStanding += Enemy.GetTotalSocialStanding();
				FinalValuesDecided?.Invoke(SocialStanding, Player.MentalCapacity);
				CombatWon?.Invoke(true);
				return;
			}
		}

		public async void EnemyAttack()
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
				if (Player.MentalCapacity <= 0)
				{
					FinalValuesDecided?.Invoke(SocialStanding, 0);
					CombatWon?.Invoke(false);
				}
			}
			Enemy.IncreaseEnthusiasmFor(chosenTopicName);
			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);
			await _encounterScene.UpdateConversationInterestMax();

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

		public override void _ExitTree()
		{
			_encounterScene.SetupCompleted -= StartCombat;
			EncounterScene.PlayerTurnComplete -= EnemyAttack;
			EnemyTurnComplete -= SetupNewAttack;
			AttackCard.AttackSelected -= PlayerAttack;
		}
	}
}
