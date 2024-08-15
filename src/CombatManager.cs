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
		private float _mentalCapacity = 10;
        private float MentalCapacity
        {
			get{return _mentalCapacity;}
			set
			{
				if(value < 0){
					_mentalCapacity = 0;
				}else if(value > 10){
					_mentalCapacity = 10;
				}else{
					_mentalCapacity = value;
				}	
			}
		}
		private TopicName _currentTopic;
		private TopicName _nextTopic;
		private EncounterEnemy _enemy;
		private float _maxConversationInterest;
		private float _conversationInterest;
		private float ConversationInterest{
			get{return _conversationInterest;}
			set
			{
				if(value < 0){
					_conversationInterest = 0;
				}else if(value > _maxConversationInterest){
					_conversationInterest = _maxConversationInterest;
				}else{
					_conversationInterest = value;
				}
			}
		}
		private Array<PlayerAttack> _currentAttacks = new();
		private Array<PlayerAttack> _allPlayerAttacks;
		private Array<PlayerAttack> _attackPool;
		private PlayerAttack _selectedAttack;
		private bool _isFirstTurn = true;

		public override void _Ready()
		{
			_allPlayerAttacks = GameManager.AvailableAttacks;
			_socialBatteryTemp = GameManager.SocialBattery - 10;
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
			_enemy = new(_encounterScene.CurrentEnemy);
			_maxConversationInterest = _enemy.ConversationInterest;
			ConversationInterest = _maxConversationInterest;
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
			if (MentalCapacity <= 0)
			{
				CombatEnded?.Invoke();
				//GameManager.SocialStandingOverall += SocialStandingCombat;
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
			TopicName conversationTopic = attackButton.BoundTopic;
			_selectedAttack = attackButton.BoundAttack;

			if (_enemy.HasLike(conversationTopic))
			{
				ConversationInterest += _selectedAttack.ConversationInterestChangeLike;
				MentalCapacity += _selectedAttack.MentalCapacityChangeLike;
				_socialBatteryTemp += _selectedAttack.SocialBatteryChangeLike;
			}
			else
			{
				ConversationInterest += _selectedAttack.ConversationInterestChangeDislike;
				MentalCapacity += _selectedAttack.MentalCapacityChangeDislike;
				_socialBatteryTemp += _selectedAttack.SocialBatteryChangeDislike;
			}

			_encounterScene.PlayCombatAnimation(_selectedAttack, conversationTopic);
			_encounterScene.UpdateUI(GameManager.SocialBattery, /*SocialStandingCombat*/0, MentalCapacity, ConversationInterest);
			GD.Print("Player Attacks!");
			if (ConversationInterest <= 0)
			{
				CombatEnded?.Invoke();
				return;
			}
		}

		public void EnemyAttack()
		{
			EnemyAttack enemyAttack = _enemy.ChooseAttack();;
			_socialBatteryTemp += enemyAttack.SocialBatteryChange;
			MentalCapacity += enemyAttack.MentalCapacityChange;
			_encounterScene.PlayCombatAnimation(enemyAttack);
			_encounterScene.UpdateUI(_socialBatteryTemp, /*SocialStandingCombat*/0, MentalCapacity, ConversationInterest);
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
