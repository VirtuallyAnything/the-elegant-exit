using Godot;
using System;

namespace tee
{
	[GlobalClass]
	public partial class EnemyAttack : Attack
	{
		private float _mentalCapacityChange;
		private int _socialBatteryChange;
		private TopicName _topic;
		[Export] 
		private string _quote;
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
		public TopicName Topic{
			get;
		}
	}
}
