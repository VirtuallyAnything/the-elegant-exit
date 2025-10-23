using Godot;
using System;

namespace tee
{
	public partial class PartyPlayer : PartyCharacter
	{
		[Export] private PlayerData _data;
		public PlayerData Data
		{
			get{return _data;}
		}

		public override void _Ready()
		{
			base._Ready();
		}
	}
}
