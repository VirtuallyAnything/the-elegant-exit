using Godot;
using System;
using System.Text.Json.Serialization;

namespace tee
{
	[GlobalClass]
	public partial class EnemyAttack : Attack
	{
		private int _mentalCapacityDamage;
		private int _socialBatteryChange;
		private string _quote;
		[Export]
		public int SocialBatteryChange
		{
			get { return _socialBatteryChange; }
			set { _socialBatteryChange = value; }
		}
		[Export]
		public int MentalCapacityDamage
		{
			get { return _mentalCapacityDamage; }
			set { _mentalCapacityDamage = value; }
		}
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public TopicName Topic
		{
			get; set;
		}
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public QuoteType Type
		{
			get; set;
		}
		[Export]
		public string Quote
		{
			get { return _quote; }
			set { _quote = value; }
		}
		public string DialogueID
		{
			get; set;
		}
		public bool NeedsTransition
		{
			get; set;
		}
	}
}
