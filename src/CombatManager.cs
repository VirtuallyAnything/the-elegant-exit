using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void CombatEventHandler();
	public partial class CombatManager : Node
	{
		public static event CombatEventHandler CombatEnded;
		[Export] private EncounterScene _encounterScene;
		private float _mentalCapacity = 20;
		private float _socialStandingCombat;
		private EnemyData _enemyData;
		private float _conversationInterest;
		private Array<PlayerAttack> _currentAttacks = new();
		private Array<PlayerAttack> _allPlayerAttacks;
		private Array<PlayerAttack> _attackPool;
		private PlayerAttack _selectedAttack;
		private bool _isFirstTurn = true;

		public override void _Ready()
		{
			_allPlayerAttacks = GameManager.AvailableAttacks;
			_attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
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
				randomAttack = ChooseRandomPlayerAttack();
				_currentAttacks.Add(randomAttack);
				_encounterScene.AttackButtons[i].SetupButton(randomAttack);
			}
			_enemyData = _encounterScene.CurrentEnemy;
			_conversationInterest = _enemyData.ConversationInterest;
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
			if (_mentalCapacity <= 0)
			{
				CombatEnded?.Invoke();
				GameManager.SocialStandingOverall += _socialStandingCombat;
				return;
			}
			_currentAttacks.Remove(_selectedAttack);
			PlayerAttack newAttack = ChooseRandomPlayerAttack();
			_currentAttacks.Add(newAttack);
			_encounterScene.SetupSelectedButton(newAttack);
		}

		public void PlayerAttack(AttackButton attackButton)
		{
			_isFirstTurn = false;
			ConversationTopic conversationTopic = attackButton.BoundTopic;
			_selectedAttack = attackButton.BoundAttack;

			if (_enemyData.TopicPreferences.Contains(conversationTopic))
			{
				_socialStandingCombat += _selectedAttack.SocialStandingChangeLike;
				_conversationInterest += _selectedAttack.ConversationInterestChangeLike;
				_mentalCapacity += _selectedAttack.MentalCapacityChangeLike;
				GameManager.SocialBattery += _selectedAttack.SocialBatteryChangeLike;
			}
			else
			{
				_socialStandingCombat += _selectedAttack.SocialStandingChangeDislike;
				_conversationInterest += _selectedAttack.ConversationInterestChangeDislike;
				_mentalCapacity += _selectedAttack.MentalCapacityChangeDislike;
				GameManager.SocialBattery += _selectedAttack.SocialBatteryChangeDislike;
			}

			_encounterScene.PlayCombatAnimation(_selectedAttack, conversationTopic);
			_encounterScene.UpdateUI(GameManager.SocialBattery, _socialStandingCombat, _mentalCapacity, _conversationInterest);
			GD.Print("Player Attacks!");

		}

		public void EnemyAttack()
		{
			if (_conversationInterest <= 0)
			{
				CombatEnded?.Invoke();
				GameManager.SocialStandingOverall += _socialStandingCombat;
				return;
			}
			EnemyAttack enemyAttack = _enemyData.EnemyAttacks.PickRandom();
			GameManager.SocialBattery += enemyAttack.SocialBatteryChange;
			_mentalCapacity += enemyAttack.MentalCapacityChange;
			_encounterScene.PlayCombatAnimation(enemyAttack);
			_encounterScene.UpdateUI(GameManager.SocialBattery, _socialStandingCombat, _mentalCapacity, _conversationInterest);
		}

		public PlayerAttack ChooseRandomPlayerAttack()
		{
			if (_attackPool.Count == 0)
			{
				_attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
				foreach (PlayerAttack attack in _currentAttacks)
				{
					_attackPool.Remove(attack);
				}
			}
			PlayerAttack randomAttack = _attackPool.PickRandom();
			_attackPool.Remove(randomAttack);
			return randomAttack;
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
