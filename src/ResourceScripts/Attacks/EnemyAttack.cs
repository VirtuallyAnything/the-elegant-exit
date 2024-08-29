using Godot;
using System;

namespace tee
{
	[GlobalClass]
	public partial class EnemyAttack : Attack
	{
		private float _mentalCapacityDamage;
		private int _socialBatteryChange;
		[Export]
		private TopicName _topic;
		private string _quote;
		[Export]
		public int SocialBatteryChange
		{
			get { return _socialBatteryChange; }
			set { _socialBatteryChange = value; }
		}
		[Export]
		public float MentalCapacityDamage
		{
			get { return _mentalCapacityDamage; }
			set { _mentalCapacityDamage = value; }
		}
		public TopicName Topic{
			get{return _topic;}
		}
		[Export]
		public string Quote{
			get{return _quote;}
			set{_quote = value;}
		}
	}
}
