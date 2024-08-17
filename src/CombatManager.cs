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
		private EnemyInterest _currentTopic;
		private EnemyInterest _nextTopic;

		private EncounterEnemy _enemy;
		private Preference _preferenceForCurrentTopic;
		private bool _isBlockNextEnemyAttack;


		private PlayerAttack _selectedAttack;
		private bool _isFirstTurn = true;

		public Preference PreferenceForCurrentTopic
		{
			get { return _preferenceForCurrentTopic; }
		}

		public override void _Ready()
		{
			_player = new(GameManager.AvailableAttacks);
			_socialBatteryTemp = GameManager.SocialBattery - 10;
			_encounterScene.SetupCompleted += StartCombat;
			EncounterScene.PlayerTurnAnimationComplete += EnemyAttack;
			EncounterScene.EnemyTurnAnimationComplete += SetupNewAttack;
			AttackButton.OnButtonPressed += PlayerAttack;
		}

		public void StartCombat()
		{
			PlayerAttack randomAttack;
			for (int i = 0; i <= 2; i++)
			{
				randomAttack = _player.ChooseRandomAttack();

				_encounterScene.AttackButtons[i].SetupButton(randomAttack);
			}
			_enemy = new(_encounterScene.CurrentEnemy);
			_encounterScene.DisableAttackButtons(true);
			EnemyAttack();
		}

		private void SetupNewAttack()
		{
			if (_isFirstTurn)
			{
				_encounterScene.DisableAttackButtons(false);
				return;
			}
			if (_player.MentalCapacity <= 0)
			{
				CombatEnded?.Invoke();
				//GameManager.SocialStandingOverall += SocialStandingCombat;
				return;
			}

			PlayerAttack newAttack = _player.SwapAttackOut(_selectedAttack);
			_encounterScene.SetupSelectedButton(newAttack);
		}

		public void PlayerAttack(AttackButton attackButton)
		{
			_isFirstTurn = false;
			TopicName conversationTopic = attackButton.BoundTopic;
			_selectedAttack = attackButton.BoundAttack;
			_preferenceForCurrentTopic = _enemy.GetPreferenceFor(conversationTopic);

			int conversationInterestBonusDamage = 0;
			_socialBatteryTemp += _selectedAttack.SocialBatteryChange;
			switch (_preferenceForCurrentTopic)
			{
				case Preference.Like:
					conversationInterestBonusDamage -= 1;
					break;
				case Preference.Dislike:
					conversationInterestBonusDamage += 1;
					break;
			}
			_enemy.ConversationInterest -= conversationInterestBonusDamage + _selectedAttack.ConversationInterestChange;

			_encounterScene.PlayCombatAnimation(_selectedAttack, conversationTopic);
			_encounterScene.UpdateUI(GameManager.SocialBattery, /*SocialStandingCombat*/0, _player.MentalCapacity, _enemy.ConversationInterest);
			GD.Print("Player Attacks!");
			if (_enemy.ConversationInterest <= 0)
			{
				CombatEnded?.Invoke();
				return;
			}
		}

		public void EnemyAttack()
		{
			EnemyAttack enemyAttack = _enemy.ChooseAttack();
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

		public override void _ExitTree()
		{
			//NumberedButton.OnButtonPressed -= PlayerAttack;
			_encounterScene.SetupCompleted -= StartCombat;
			EncounterScene.PlayerTurnAnimationComplete -= EnemyAttack;
			EncounterScene.EnemyTurnAnimationComplete -= SetupNewAttack;
			AttackButton.OnButtonPressed -= PlayerAttack;
		}
	}
}
