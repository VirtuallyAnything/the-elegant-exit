using Godot;
using System;

namespace tee
{
	public partial class EnemyAttack : Attack
	{
		private float _mentalCapacityChange;
		private int _socialBatteryChange;

		[Export]
		public int SocialBatteryChange
		{
			get { return _socialBatteryChange; }
			set { _socialBatteryChange = value; }
		}
		[Export]
		public float MentalCapacityChange
		{
			get { return _mentalCapacityChange; }
			set { _mentalCapacityChange = value; }
		}
	}
}
