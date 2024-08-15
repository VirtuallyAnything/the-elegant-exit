using Godot;
using System;

namespace tee
{
	[Tool]
	public partial class PlayerAttack : Attack
	{
		private float _socialStandingChangeLike,
			_socialStandingChangeDislike;
		private float _mentalCapacityChangeLike;
		private float _mentalCapacityChangeDislike;
		private float _conversationInterestChangeLike;
		private float _conversationInterestChangeDislike;
		private int _socialBatteryChangeLike;
		private int _socialBatteryChangeDislike;
		private bool _enableTopicChoice;
		private bool _isFromItem;
		private Godot.Collections.Array<TopicName> _unlockedTopics = new();
		[Export]
		private Godot.Collections.Dictionary<TopicName, string> _topicRelatedQuotes = new(){
			{TopicName.None, ""},
			{TopicName.Sport, ""},
			{TopicName.Lifestyle, ""},
			{TopicName.Economy, ""},
			{TopicName.Politics, ""},
			{TopicName.Art, ""},
			{TopicName.Gossip, ""},
			{TopicName.Food, ""},
			{TopicName.Drink, ""}
		};

		[Export]
		public float SocialStandingChangeLike
		{
			get { return _socialStandingChangeLike; }
			set { _socialStandingChangeLike = value; }
		}
		[Export]
		public float SocialStandingChangeDislike
		{
			get { return _socialStandingChangeDislike; }
			set { _socialStandingChangeDislike = value; }
		}
		[Export]
		public float MentalCapacityChangeLike
		{
			get { return _mentalCapacityChangeLike; }
			set { _mentalCapacityChangeLike = value; }
		}
		[Export]
		public float MentalCapacityChangeDislike
		{
			get { return _mentalCapacityChangeDislike; }
			set { _mentalCapacityChangeDislike = value; }
		}
		[Export]
		public float ConversationInterestChangeLike
		{
			get { return _conversationInterestChangeLike; }
			set { _conversationInterestChangeLike = value; }
		}
		[Export]
		public float ConversationInterestChangeDislike
		{
			get { return _conversationInterestChangeDislike; }
			set { _conversationInterestChangeDislike = value; }
		}
		[Export]
		public int SocialBatteryChangeLike
		{
			get { return _socialBatteryChangeLike; }
			set { _socialBatteryChangeLike = value; }
		}
		[Export]
		public int SocialBatteryChangeDislike
		{
			get { return _socialBatteryChangeDislike; }
			set { _socialBatteryChangeDislike = value; }
		}
		[Export]
		public bool EnableTopicChoice
		{
			get { return _enableTopicChoice; }
			set { _enableTopicChoice = value; }
		}
		[Export]
		public bool IsFromItem
		{
			get { return _isFromItem; }
			set { _isFromItem = value; }
		}
		[Export]
		public Godot.Collections.Array<TopicName> UnlockedTopics
		{
			get { return _unlockedTopics; }
			set { _unlockedTopics = value; }
		}

		public string GetQuoteForTopic(TopicName topic){
			return _topicRelatedQuotes[topic];
		}
	}
}
