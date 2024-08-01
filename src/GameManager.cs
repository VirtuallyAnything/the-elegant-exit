using Godot;
using System;

namespace tee
{
	public delegate void GameHandler();
	public partial class GameManager : Node
	{
		public static event GameHandler GameOver;
		private static Player _player;
		private static Godot.Collections.Array<PlayerAttack> _availableAttacks;
		private int[] _partyMembers;
		private static EnemyData _currentEnemy;
		public static EnemyData CurrentEnemy{
			get{return _currentEnemy;}
			set{_currentEnemy = value;}
		}
		private static float _socialStandingOverall;
		private static int _socialBattery = 30;

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
				if(value > 30){
					_socialBattery = 30;
				}else if(value <= 0){
					_socialBattery = 0;
					GameOver?.Invoke();
				}else{
					_socialBattery = value;
				}
			}
		}

		public static float SocialStandingOverall{
			get{return _socialStandingOverall;}
			set{_socialStandingOverall = value;}
		}
		public static Player Player{
			get{return _player;}
			set{_player = value;}
		}
		
		public override void _Ready()
		{	
		}

		public static void SetupGame(){
			_availableAttacks = _player.Data.AvailableAttacks;
			_socialStandingOverall = _player.Data.SocialStandingOverall;
			_socialBattery = _player.Data.SocialBattery;
		}
	}
}
