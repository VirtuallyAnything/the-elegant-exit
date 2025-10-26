using System.Threading.Tasks;
using Godot;

namespace tee
{
	public delegate Task GameHandler(SceneName sceneName);
	public partial class GameManager : Node
	{
		public static event GameHandler GameOver;
		[Export] private GameTimer _gameTimer;
		[Export] private PartyPlayer _player;
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
		private static int _socialBattery;
		private static int _maxSocialBattery;
		private static bool _isFirstEncounter = true;
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
				if (value > _maxSocialBattery)
				{
					_socialBattery = _maxSocialBattery;
				}
				else if (value <= 0)
				{
					_socialBattery = 0;
					EndGameAsLost();
				}
				else
				{
					_socialBattery = value;
				}
			}
		}
		public static bool IsFirstEncounter
		{
			get { return _isFirstEncounter; }
		}

		public override void _Ready()
		{
			AvailableAttacks = _player.Data.AvailableAttacks;
			_socialStandingOverall = _player.Data.SocialStandingOverall;
			_maxSocialBattery = _player.Data.SocialBattery;
			SocialBattery = _maxSocialBattery;
			CombatManager.FinalValuesDecided += UpdateStats;
			CombatManager.CombatWon += SetIsFirstEncounter;
			_gameTimer.Timeout += EndGameAsLost;
		}

		private void SetIsFirstEncounter(EncounterOutcome outcome)
		{
			_isFirstEncounter = false;
			CombatManager.CombatWon -= SetIsFirstEncounter;
		}

		private static void UpdateStats(int socialStanding, int socialBattery)
		{
			_socialStandingOverall += socialStanding;
			SocialBattery += socialBattery;
		}

		public static void EndGameAsLost()
		{
			GameOver?.Invoke(SceneName.GameOver);
		}

		public static int GetScore()
		{
			return _socialStandingOverall;
		}

		public override void _ExitTree()
		{
			CombatManager.FinalValuesDecided -= UpdateStats;
			_gameTimer.Timeout -= EndGameAsLost;
			QueueFree();
		}
	}
}
