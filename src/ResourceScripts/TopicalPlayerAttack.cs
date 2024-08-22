using Godot;
using System;

namespace tee
{
	[GlobalClass]
	public partial class TopicalPlayerAttack : PlayerAttack
	{
		private bool _isFromItem;
		private Godot.Collections.Array<TopicName> _unlockedTopics = new();
		public TopicName SelectedTopicName{
			get; set;
		}
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
		public Godot.Collections.Array<TopicName> UnlockedTopics
		{
			get { return _unlockedTopics; }
			set { _unlockedTopics = value; }
		}

		public override string GetQuote(){
			return _topicRelatedQuotes[SelectedTopicName];
		}
	}
}
