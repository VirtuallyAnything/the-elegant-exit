using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void CombatEventHandler();
	public partial class CombatManager : Node
	{
		public static event CombatEventHandler CombatEnded;
		[Export] private EncounterScene _encounterScene;

		private int _socialBatteryTemp;
		private EncounterPlayer _player;
		private int _conversationInterestDamage;
		private TopicName _playerCurrentTopicName;
		private TopicName _playerLastTopicName;
		private PlayerAttack _selectedAttack;
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
			_socialBatteryTemp = GameManager.SocialBattery - 10;
			_encounterScene.SetupCompleted += StartCombat;
			EncounterScene.PlayerTurnAnimationComplete += EnemyAttack;
			EncounterScene.EnemyTurnAnimationComplete += SetupNewAttack;
			AttackCard.AttackSelected += PlayerAttack;
		}

		public void StartCombat()
		{
			PlayerAttack randomAttack;
			for (int i = 0; i <= 2; i++)
			{
				randomAttack = _player.ChooseRandomAttack();

				_encounterScene.AttackCardContainer.AddNewAttackCard(randomAttack);
			}
			_enemy = new(_encounterScene.CurrentEnemy);
			_encounterScene.AttackCardContainer.DisableInput();
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

		public void PlayerAttack(AttackCard attackCard)
		{
			_isFirstTurn = false;
			_playerLastTopicName = _playerCurrentTopicName;
			TopicName topicOfAttack = attackCard.BoundTopic;
			_selectedAttack = attackCard.BoundAttack;

			int conversationInterestBonusDamage = 0;
			if (topicOfAttack != TopicName.None)
			{
				_playerCurrentTopicName = topicOfAttack;
				_preferenceForCurrentTopic = _enemy.GetPreferenceFor(topicOfAttack);
				if (!_isIgnoreCIBonusDamage)
				{
					switch (_preferenceForCurrentTopic)
					{
						case Preference.Like:
							conversationInterestBonusDamage -= 1;
							break;
						case Preference.Dislike:
							conversationInterestBonusDamage += 1;
							if (_playerCurrentTopicName == _playerLastTopicName)
							{
								_enemy.Enrage(_playerCurrentTopicName);
							}
							if (_player.DiscoveredEnemyPreferences.ContainsKey(_playerCurrentTopicName))
							{
								_socialBatteryTemp -= 1;
							}
							break;
					}
				}
			}

			_player.Resolve(_selectedAttack, this);
			_enemy.ReactTo(topicOfAttack);
			if (!_player.DiscoveredEnemyPreferences.ContainsKey(_playerCurrentTopicName))
			{
				_player.DiscoveredEnemyPreferences.Add(_playerCurrentTopicName, _preferenceForCurrentTopic);
			}
			_enemy.ConversationInterest -= conversationInterestBonusDamage + ConversationInterestDamage;
			_isIgnoreCIBonusDamage = false;

			_encounterScene.PlayCombatAnimation(_selectedAttack);
			_encounterScene.UpdateUI(_socialBatteryTemp, /*SocialStandingCombat*/0, _player.MentalCapacity, _enemy.ConversationInterest);
			GD.Print($"Player attacks and does {conversationInterestBonusDamage + ConversationInterestDamage} damage to CI.");
			GD.Print($"New Enemy CI: {_enemy.ConversationInterest}");
			if (_enemy.ConversationInterest <= 0)
			{
				CombatEnded?.Invoke();
				return;
			}
		}

		public void EnemyAttack()
		{
			TopicName chosenTopicName;
			if (NextTopicName != TopicName.None)
			{
				chosenTopicName = NextTopicName;
				NextTopicName = TopicName.None;
			}
			else
			{
				chosenTopicName = _enemy.ChooseTopic();
			}

			EnemyAttack enemyAttack = _enemy.ChooseAttack(chosenTopicName);

			if (_isBlockNextEnemyAttack)
			{
				_isBlockNextEnemyAttack = false;
			}
			else
			{
				_socialBatteryTemp += enemyAttack.SocialBatteryChange;
				_player.MentalCapacity += enemyAttack.MentalCapacityChange;
			}

			_encounterScene.PlayCombatAnimation(enemyAttack);
			_encounterScene.UpdateUI(_socialBatteryTemp, /*SocialStandingCombat*/0, _player.MentalCapacity, _enemy.ConversationInterest);
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
			EncounterScene.PlayerTurnAnimationComplete -= EnemyAttack;
			EncounterScene.EnemyTurnAnimationComplete -= SetupNewAttack;
			AttackCard.AttackSelected -= PlayerAttack;
		}
	}
}
