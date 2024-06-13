using Godot;
using System;

namespace tee
{
	public delegate void GameHandler();
	public partial class GameManager : Node
	{
		public static event GameHandler GameOver;
		private static PlayerData _playerData;
		private static Godot.Collections.Array<PlayerAttack> _availableAttacks;
		private int[] _partyMembers;
		private static float _socialStandingOverall;
		private static int _socialBattery;

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
				_socialBattery = value;
				if(_socialBattery <= 0){
					GameOver?.Invoke();
				} 
			}
		}

		public static float SocialStandingOverall{
			get{return _socialStandingOverall;}
			set{_socialStandingOverall = value;}
		}
		public override void _Ready()
		{
			_playerData = GD.Load<PlayerData>("res://GameDataResources/Attacks/PlayerData.tres");
			_availableAttacks = _playerData.AvailableAttacks;
			_socialStandingOverall = _playerData.SocialStandingOverall;
			_socialBattery = _playerData.SocialBattery;
		}
	}
}
