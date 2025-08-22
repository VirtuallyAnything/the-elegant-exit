using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public delegate void GameHandler();
	public partial class GameManager : Node
	{
		public static event GameHandler GameOver;
		private static PartyPlayer _player;
		private static Godot.Collections.Array<PlayerAttack> _availableAttacks;
		private int[] _partyMembers;
		private static EnemyData _currentEnemy;
		public static EnemyData CurrentEnemy
		{
			get { return _currentEnemy; }
			set { _currentEnemy = value; }
		}
		private static int _socialStandingOverall;
		private static int _socialBattery = 100;
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

		public static int SocialStandingOverall
		{
			get { return _socialStandingOverall; }
			set { _socialStandingOverall = value; }
		}
		public static PartyPlayer Player
		{
			get { return _player; }
			set { _player = value; }
		}

		public override void _Ready()
		{
		}

		public static void SetupGame()
		{
			_availableAttacks = _player.Data.AvailableAttacks;
			_socialStandingOverall = _player.Data.SocialStandingOverall;
			_socialBattery = _player.Data.SocialBattery;
		}

		public static void EndGame()
		{
			GameOver?.Invoke();
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			QueueFree();
		}
	}
}
