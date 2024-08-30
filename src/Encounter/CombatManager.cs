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
		}
		public TopicName PlayerLastTopicName{
			get{return _playerLastTopicName;}
		}
		public PlayerAttack SelectedAttack{
			get{return _selectedAttack;}
		}
		public TopicName NextTopicName
		{
			get { return _nextTopicName; }
			set
			{
				if (_enemy.GetPreferenceFor(value) != Preference.Dislike)
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
			EnemyAttack();
		}

		private void SetupNewAttack()
		{
			if (_isFirstTurn)
			{
				_encounterScene.AttackCardContainer.EnableInput();
				return;
			}
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
			ConversationInterestDamage = 0;
			_playerLastTopicName = _playerCurrentTopicName;

			// Set topic of attack to that of the player attack or none
			TopicName topicOfAttack = TopicName.None;
			if(playerAttack is TopicalPlayerAttack topicalPlayerAttack){
				topicOfAttack = topicalPlayerAttack.SelectedTopicName;
			};
			_selectedAttack = playerAttack;

			int conversationInterestBonusDamage = 0;
			if (topicOfAttack != TopicName.None)
			{
				//Only change the current Topic if the TopicName isn't none
				_playerCurrentTopicName = topicOfAttack;
				_preferenceForCurrentTopic = Enemy.GetPreferenceFor(topicOfAttack);
				if (!_isIgnoreCIBonusDamage)
				{
					switch (_preferenceForCurrentTopic)
					{
						// Apply damage modifiers for Preferences
						case Preference.Like:
							conversationInterestBonusDamage -= 1;
							break;
						case Preference.Dislike:
							conversationInterestBonusDamage += 1;
							if (_playerCurrentTopicName == _playerLastTopicName)
							{
								Enemy.Enrage(_playerCurrentTopicName);
							}
							// Subtract one social Battery if the player knowingly talks about a disliked topic
							if (_player.DiscoveredEnemyPreferences.ContainsKey(_playerCurrentTopicName))
							{
								GameManager.SocialBattery -= 1;
							}
							break;
					}
				}
			}

			// Set the base attack damage and resolve bonus effects if there are any
			_player.Resolve(_selectedAttack, this);
			Enemy.ReactTo(topicOfAttack);

			// Add newly discovered preference
			if (!_player.DiscoveredEnemyPreferences.ContainsKey(_playerCurrentTopicName))
			{
				_player.DiscoveredEnemyPreferences.Add(_playerCurrentTopicName, _preferenceForCurrentTopic);
				_preferenceDisplay.UpdatePreference(
					_playerCurrentTopicName, _preferenceForCurrentTopic);
			}
			// Actually subtract the damages from Conversation Interest
			Enemy.ConversationInterest -= conversationInterestBonusDamage + ConversationInterestDamage;

			_isIgnoreCIBonusDamage = false;

			await _encounterScene.PlayDialogAnimation(_selectedAttack);
			await _encounterScene.PlayAnimationsForAttack(_selectedAttack, conversationInterestBonusDamage);
			await _encounterScene.UpdateConversationInterestMax();

			if(topicOfAttack != TopicName.None){
				_preferenceDisplay.UpdateEnthusiasm(_playerCurrentTopicName, Enemy.GetEnthusiasmLevelFor(_playerCurrentTopicName));
				_encounterScene.UpdateTopic(topicOfAttack);
			}
			
			_encounterScene.UpdateAnnoyance(Enemy.Annoyance);
			_encounterScene.UpdateConversationInterestModifiers(Enemy.ConversationInterestModifierAnnoyance, Enemy.ConversationInterestModifierEnthusiasm);
			GD.Print($"Player attacks and does {conversationInterestBonusDamage + ConversationInterestDamage} damage to CI.");
			GD.Print($"New Enemy CI: {Enemy.ConversationInterest}/{Enemy.ConversationInterestMax}");
			if (Enemy.ConversationInterest <= 0)
			{
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
			_encounterScene.PlayDialogAnimation(enemyAttack);

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
			await _encounterScene.UpdateConversationInterestMax();
			Enemy.IncreaseEnthusiasmFor(chosenTopicName);
			_preferenceDisplay.UpdateEnthusiasm(chosenTopicName, Enemy.GetEnthusiasmLevelFor(chosenTopicName));
			_encounterScene.UpdateTopic(chosenTopicName);
			GD.Print("Enemy selects Topic");
			_encounterScene.UpdateConversationInterestModifiers(_enemy.ConversationInterestModifierAnnoyance, _enemy.ConversationInterestModifierEnthusiasm);
			
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
			_enemy.IgnoreNextAnnoyance = true;
		}

		public void IgnoreNextEnthusiasm()
		{
			_enemy.IgnoreNextEnthusiasm = true;
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
