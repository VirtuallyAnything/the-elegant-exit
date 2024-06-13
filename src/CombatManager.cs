using Godot;
using Godot.Collections;

namespace tee
{
	public partial class CombatManager : Node
	{
		//public static event CombatEventHandler CombatStateChanged;

		[Export] private EncounterScene _encounterScene;
		[Export] private PlayerData _playerData;
		private float _mentalCapacity;
		private float _socialStandingCombat;
		private EnemyData _enemyData;
		private Array<PlayerAttack> _currentAttacks = new();
		private Array<PlayerAttack> _allPlayerAttacks;
		private Array<PlayerAttack> _attackPool;
		private PlayerAttack _selectedAttack;

		public override void _Ready()
		{
			_allPlayerAttacks = _playerData.AvailableAttacks;
			_attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
			_encounterScene.SetupCompleted += StartCombat;
			EncounterScene.PlayerTurnAnimationComplete += EnemyAttack;
			EncounterScene.EnemyTurnAnimationComplete += SetupNewAttack;
			AttackButton.OnButtonPressed += SetSelectedAttack;
			TopicButton.OnButtonPressed += PlayerAttack;
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
			//UpdateCombatUI()
		}

		private void SetSelectedAttack(AttackButton attackButton)
		{
			_selectedAttack = attackButton.BoundAttack;
		}

		private void SetupNewAttack()
		{
			_currentAttacks.Remove(_selectedAttack);
			PlayerAttack newAttack = ChooseRandomPlayerAttack();
			_currentAttacks.Add(newAttack);
			_encounterScene.SetupSelectedButton(newAttack);
		}

		public void PlayerAttack(ConversationTopic conversationTopic)
		{

			if (_selectedAttack.EnableTopicChoice)
			{
				for (int i = 0; i < _enemyData.TopicPreferences.Count; i++)
				{
					if (_enemyData.TopicPreferences.Contains(conversationTopic))
					{
						_socialStandingCombat += _selectedAttack.SocialStandingChangeLike;
					}
					else
					{
						_socialStandingCombat += _selectedAttack.SocialStandingChangeDislike;
					}
				}
			}

			_encounterScene.PlayCombatAnimation(_selectedAttack);
			GD.Print("Player Attacks!");

		}

		public void EnemyAttack()
		{
			EnemyAttack enemyAttack = _enemyData.EnemyAttacks.PickRandom();
			GameManager.PlayerData.SocialBattery -= enemyAttack.SocialBatteryChange;
			_mentalCapacity -= enemyAttack.MentalCapacityChange;
			_encounterScene.PlayCombatAnimation(enemyAttack);
		}

		public PlayerAttack ChooseRandomPlayerAttack()
		{

			if (_attackPool.Count == 0)
			{
				_attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
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
			AttackButton.OnButtonPressed -= SetSelectedAttack;
			TopicButton.OnButtonPressed -= PlayerAttack;
		}
	}
}
