using Godot;
using System;

namespace tee
{
	public partial class GameManager : Node
	{
		private static PlayerData _playerData;
		public static PlayerData PlayerData
		{
			get { return _playerData; }
			set { _playerData = value; }
		}
		public override void _Ready()
		{
			_playerData = GD.Load<PlayerData>("res://GameDataResources/Attacks/PlayerData.tres");
		}
	}
}
