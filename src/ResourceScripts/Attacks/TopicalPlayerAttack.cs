using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace tee
{
	public delegate void TopicUpdateHandler(List<TopicName> updatedTopics);
	[GlobalClass]
	public partial class TopicalPlayerAttack : PlayerAttack
	{
		private bool _isFromItem;
		[Export] private Array<TopicName> _unlockedTopics = new();
		private List<TopicName> _availableTopics;
		public TopicName SelectedTopicName
		{
			get; set;
		}
		[Export]
		private Array<TopicDialogue> _topicRelatedQuotes = new();
		public Array<TopicName> UnlockedTopics
		{
			get { return _unlockedTopics; }
		}
		public List<TopicName> AvailableTopics
        {
            get{ return _availableTopics; }
        }
		
		public override string GetQuote()
		{
			foreach (TopicDialogue topicDialogue in _topicRelatedQuotes)
			{
				if (topicDialogue.TopicName == SelectedTopicName)
				{
					return topicDialogue.Text;
				}
			}
			return "";
		}

		public void UpdateAvailableTopics(CombatManager combatManager)
		{
			_availableTopics = new(UnlockedTopics);
			BonusEffect?.GetValidTopics(combatManager, ref _availableTopics);
		}
	}
}
