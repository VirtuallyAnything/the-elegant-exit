using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void CombatEventHandler();
	public partial class CombatManager : Node
	{
		public static event CombatEventHandler CombatEnded;
		public static event CombatEventHandler CombatLost;
		public static event CombatEventHandler EnemyTurnComplete;
		[Export] private EncounterScene _encounterScene;
		[Export] private PreferenceDisplay _preferenceDisplay;

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
				};
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
			_encounterScene.UpdateAnnoyance(Enemy.Annoyance);
			EnemyAttack();
		}

		private void SetupNewAttack()
		{
			/*if (_isFirstTurn)
			{
				_encounterScene.AttackCardContainer.EnableInput();
				return;
			}*/
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
			_preferenceForCurrentTopic = Preference.Unknown;

			// Set the base attack damage
			ConversationInterestDamage = _selectedAttack.ConversationInterestDamage;
			ConversationInterestBonusDamage = 0;

			_selectedAttack.BonusEffect?.Resolve(this);

			TopicName topicOfAttack;
			if (playerAttack is TopicalPlayerAttack topicalPlayerAttack)
			{
				topicOfAttack = topicalPlayerAttack.SelectedTopicName;
				PlayerCurrentTopicName = topicOfAttack;
				_encounterScene.UpdateTopic(PlayerCurrentTopicName);
				_preferenceForCurrentTopic = Enemy.GetPreferenceFor(topicOfAttack);

				if (!_isIgnoreCIBonusDamage)
				{
					switch (_preferenceForCurrentTopic)
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
			}
			else
			{
				topicOfAttack = TopicName.None;
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
			if (PlayerCurrentTopicName != TopicName.None && !_player.DiscoveredEnemyPreferences.ContainsKey(PlayerCurrentTopicName))
			{
				_player.DiscoveredEnemyPreferences.Add(PlayerCurrentTopicName, _preferenceForCurrentTopic);
				_preferenceDisplay.UpdatePreference(
					PlayerCurrentTopicName, _preferenceForCurrentTopic);
			}
			// Actually subtract the damages from Conversation Interest
			Enemy.ConversationInterest -= ConversationInterestBonusDamage + ConversationInterestDamage;
			GD.Print($"Player attacks with {playerAttack.AttackName} and does {ConversationInterestBonusDamage + ConversationInterestDamage} damage to CI.");
			GD.Print($"New Enemy CI: {Enemy.ConversationInterest}/{Enemy.ConversationInterestMax}");
			_isIgnoreCIBonusDamage = false;
			
			if (topicOfAttack != TopicName.None)
			{
				
			}
			//Wait for all the animations to finish
			await _encounterScene.PlayDialogAnimation(_selectedAttack);
			await _encounterScene.PlayAnimationsForAttack(_selectedAttack, ConversationInterestBonusDamage);
			await _encounterScene.UpdateConversationInterestMax();

			//if special attack with no topic occurs, the topic of the player isn't none but weather!! Should change to "topic stays that of the enemy until player attacks with a topical attack"

			if (PlayerCurrentTopicName != Enemy.CurrentTopicName)
			{
				_preferenceDisplay.UpdateEnthusiasm(Enemy.LastTopicName, Enemy.GetEnthusiasmLevelFor(Enemy.LastTopicName));
			}
			_preferenceDisplay.UpdateEnthusiasm(PlayerCurrentTopicName, Enemy.GetEnthusiasmLevelFor(PlayerCurrentTopicName));

			_encounterScene.UpdateAnnoyance(Enemy.Annoyance);
			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);
			
			if (Enemy.ConversationInterest <= 0)
			{
				GameManager.SocialBattery += (int) _player.MentalCapacity;
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
			_preferenceDisplay.UpdateEnthusiasm(chosenTopicName, Enemy.GetEnthusiasmLevelFor(chosenTopicName));
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
			//NumberedButton.OnButtonPressed -= PlayerAttack;
			_encounterScene.SetupCompleted -= StartCombat;
			EncounterScene.PlayerTurnComplete -= EnemyAttack;
			EnemyTurnComplete -= SetupNewAttack;
			AttackCard.AttackSelected -= PlayerAttack;
		}
	}
}
