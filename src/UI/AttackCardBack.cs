using Godot;
using System;

namespace tee
{
    public delegate void TopicHandler(TopicName topicName);
    public partial class AttackCardBack : AttackCardSide
    {
        private Label _attackName;
        private TopicName _boundTopic;
        private Godot.Collections.Array<TopicButton> _topicButtonsAttack = new();
        public event TopicHandler TopicPicked;

        [Export]
        public ButtonGrid TopicGrid
        {
            get; set;
        }
        public TopicName BoundTopic
        {
            get { return _boundTopic; }
            set { _boundTopic = value; }
        }

        public AttackCardBack(PlayerAttack attack)
        {
            _attackName.Text = attack.AttackName;
            if (attack is TopicalPlayerAttack topicalPlayerAttack)
            {
                foreach (TopicName conversationTopic in topicalPlayerAttack.UnlockedTopics)
                {
                    TopicButton button = new()
                    {
                        Text = conversationTopic.ToString(),
                        ConversationTopic = conversationTopic,
                        Parent = this
                    };
                    _topicButtonsAttack.Add(button);
                }
            }
        }

        public void ChildPressed(TopicButton topicButton)
        {
            TopicPicked?.Invoke(topicButton.ConversationTopic);
        }
    }
}