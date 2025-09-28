using Godot;

namespace tee
{
	public delegate void GameHandler();
	public partial class GameManager : Node
	{
		public static event GameHandler GameOver;
		private static PartyPlayer _player;
		private static Godot.Collections.Array<PlayerAttack> _availableAttacks;
		//int placeholder
		private int[] _partyMembers;
		private static EnemyData _currentEnemy;
		public static EnemyData CurrentEnemy
		{
			get { return _currentEnemy; }
			set { _currentEnemy = value; }
		}
		private static int _socialStandingOverall;
		private static int _socialBattery = 100;
		private static bool _isTutorialActive = false;
		private static bool _isFirstEncounter = false;// true;
		public static Godot.Collections.Array<PlayerAttack> AvailableAttacks
		{
			get { return _availableAttacks; }
			set { _availableAttacks = value; }
		}

		public static int SocialBattery
		{
			get { return _socialBattery; }
			set
			{
				if (value > 100)
				{
					_socialBattery = 100;
				}
				else if (value <= 0)
				{
					_socialBattery = 0;
					EndGame();
				}
				else
				{
					_socialBattery = value;
				}
			}
		}
		public static bool IsFirstEncounter
		{
			get{ return _isFirstEncounter; }
		}

		public override void _Ready()
		{
			MainScene.SetupCompleted += SetupGame;
			CombatManager.CombatWon += SetIsFirstEncounter;
		}

		private void SetIsFirstEncounter(bool outcome)
		{
			_isFirstEncounter = false;
			CombatManager.CombatWon -= SetIsFirstEncounter;
		}

		private void SetupGame(PartyPlayer player)
		{
			_player = player;
			AvailableAttacks = _player.Data.AvailableAttacks;
			_socialStandingOverall = _player.Data.SocialStandingOverall;
			SocialBattery = _player.Data.SocialBattery;
			CombatManager.FinalValuesDecided += UpdateStats;
		}

		private static void UpdateStats(int socialStanding, int socialBattery)
		{
			_socialStandingOverall += socialStanding;
			SocialBattery += socialBattery;
		}

		public static void EndGame()
		{
			GameOver?.Invoke();
		}

		public static int GetScore()
		{
			return _socialStandingOverall * _socialBattery;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			CombatManager.FinalValuesDecided -= UpdateStats;
			MainScene.SetupCompleted -= SetupGame;
			QueueFree();
		}
	}
}
