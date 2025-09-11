using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void CombatEventHandler();
	public delegate void EnthusiasmEventHandler(TopicName topicName, int enthusiasm);
	public delegate void PreferenceEventHandler(TopicName topicName, Preference preference);
	public partial class CombatManager : Node
	{
		public static event CombatEventHandler CombatEnded;
		public static event CombatEventHandler CombatLost;
		public static event CombatEventHandler EnemyTurnComplete;
		public static event EnthusiasmEventHandler EnthusiasmChanged;
		public static event PreferenceEventHandler PreferenceDiscovered;
		[Export] private EncounterScene _encounterScene;
		private EncounterPlayer _player;
		private int _conversationInterestDamage;
		private int _conversationInterestBonusDamage;
		private TopicName _playerCurrentTopicName;
		private TopicName _playerLastTopicName;
		private PlayerAttack _selectedAttack;
		private int _transferAmount = 10;
		private bool _isFirstTurn = true;

		private EncounterEnemy _enemy;
		private Preference _preferenceForCurrentTopic;
		private TopicName _nextTopicName;
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
		public TopicName PlayerCurrentTopicName
		{
			get { return _playerCurrentTopicName; }
			set { _playerCurrentTopicName = value; }
		}
		public TopicName PlayerLastTopicName
		{
			get { return _playerLastTopicName; }
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
				randomAttack = _player.ChooseRandomAttack();

				_encounterScene.AttackCardContainer.AddNewAttackCard(randomAttack);
			}
			_enemy = new(_encounterScene.CurrentEnemy);

			_encounterScene.AttackCardContainer.DisableInput();
			await _encounterScene.PlayCombatStartAnimation(_transferAmount);

			GameManager.SocialBattery -= _transferAmount;
			_player.MentalCapacity = _transferAmount;
			EnemyAttack();
		}

		private void SetupNewAttack()
		{
			if (_player.MentalCapacity <= 0)
			{
				CombatEnded?.Invoke();
				//GameManager.SocialStandingOverall += SocialStandingCombat;
				return;
			}

			PlayerAttack newAttack = _player.SwapAttackOut(_selectedAttack);
			_encounterScene.AttackCardContainer.SwapAttackCardOutFor(newAttack);
		}

		public async void PlayerAttack(PlayerAttack playerAttack)
		{
			_isFirstTurn = false;

			_selectedAttack = playerAttack;
			_playerLastTopicName = PlayerCurrentTopicName;
			PreferenceForCurrentTopic = Preference.Unknown;

			// Set the base attack damage
			ConversationInterestDamage = _selectedAttack.ConversationInterestDamage;
			ConversationInterestBonusDamage = 0;

			TopicName topicOfAttack;
			if (playerAttack is TopicalPlayerAttack topicalPlayerAttack)
			{
				topicOfAttack = topicalPlayerAttack.SelectedTopicName;
				PlayerCurrentTopicName = topicOfAttack;
				_encounterScene.UpdateTopic(PlayerCurrentTopicName);
				PreferenceForCurrentTopic = Enemy.GetPreferenceFor(topicOfAttack);
			}
			else
			{
				topicOfAttack = TopicName.None;
			}

			_selectedAttack.BonusEffect?.Resolve(this);

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
						if (PlayerCurrentTopicName == _playerLastTopicName)
						{
							Enemy.Enrage(PlayerCurrentTopicName);
						}
						// Subtract one social Battery if the player knowingly talks about a disliked topic
						if (_player.DiscoveredEnemyPreferences.ContainsKey(PlayerCurrentTopicName))
						{
							GameManager.SocialBattery -= 1;
						}
						break;
				}
			}

			if (PlayerCurrentTopicName == TopicName.Weather)
			{
				Enemy.ReactTo(PlayerCurrentTopicName);
			}
			else
			{
				Enemy.ReactTo(topicOfAttack);
			}

			// Add newly discovered preference
			if (PlayerCurrentTopicName != TopicName.None)
			{
				PreferenceDiscovered?.Invoke(PlayerCurrentTopicName, _preferenceForCurrentTopic);
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

			// Invoke events to update UI on Enthusiasm
			if (PlayerCurrentTopicName != Enemy.CurrentTopicName)
			{
				EnthusiasmChanged?.Invoke(Enemy.LastTopicName, Enemy.GetEnthusiasmLevelFor(Enemy.LastTopicName));
			}
			EnthusiasmChanged?.Invoke(PlayerCurrentTopicName, Enemy.GetEnthusiasmLevelFor(PlayerCurrentTopicName));

			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);

			if (Enemy.ConversationInterest <= 0)
			{
				GameManager.SocialBattery += (int)_player.MentalCapacity;
				GameManager.SocialStandingOverall += SocialStanding;
				GameManager.SocialStandingOverall += Enemy.Annoyance.GetTotalSocialStanding();
				CombatEnded?.Invoke();
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
				_player.MentalCapacity -= enemyAttack.MentalCapacityDamage;
				await _encounterScene.PlayAnimationsForAttack(enemyAttack);
			}
			Enemy.IncreaseEnthusiasmFor(chosenTopicName);
			EnthusiasmChanged?.Invoke(chosenTopicName, Enemy.GetEnthusiasmLevelFor(chosenTopicName));
			_encounterScene.UpdateConversationInterestModifiers(_enemy.ConversationInterestModifierAnnoyance, _enemy.ConversationInterestModifierEnthusiasm);
			await _encounterScene.UpdateConversationInterestMax();

			_encounterScene.UpdateTopic(chosenTopicName);

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
			_enemy.IsIgnoreNextAnnoyance = true;
		}

		public void IgnoreTopicSwitchAnnoyance()
		{
			_enemy.IsIgnoreTopicSwitchAnnoyance = true;
		}

		public void IgnoreNextEnthusiasm()
		{
			_enemy.IsIgnoreNextEnthusiasm = true;
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
