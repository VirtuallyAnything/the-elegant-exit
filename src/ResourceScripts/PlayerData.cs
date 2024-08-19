using Godot;
using System;
using System.Collections.Generic;

namespace tee
{
	public partial class PlayerData : Resource
	{
		[Export] private Godot.Collections.Array<PlayerAttack> _availableAttacks;
		private int[] _partyMembers;
		private int _socialStandingOverall;
		private int _socialBattery = 100;

		public Godot.Collections.Array<PlayerAttack> AvailableAttacks
		{
			get { return _availableAttacks; }
			set { _availableAttacks = value; }
		}

		public int SocialBattery
		{
			get { return _socialBattery; }
			set { _socialBattery = value; }
		}

		public int SocialStandingOverall{
			get{return _socialStandingOverall;}
			set{_socialStandingOverall = value;}
		}
	}
}
