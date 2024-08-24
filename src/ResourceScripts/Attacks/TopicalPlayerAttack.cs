using Godot;
using Godot.Collections;

namespace tee
{
	[GlobalClass]
	public partial class TopicalPlayerAttack : PlayerAttack
	{
		private bool _isFromItem;
		private Array<TopicName> _unlockedTopics = new();
		public TopicName SelectedTopicName
		{
			get; set;
		}
		[Export]
		private Array<TopicDialogue> _topicRelatedQuotes = new();
		[Export]
		public Array<TopicName> UnlockedTopics
		{
			get { return _unlockedTopics; }
			set { _unlockedTopics = value; }
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
	}
}
