using Godot;
using System;

namespace tee
{
    public delegate void TopicHandler(TopicName topicName);
    public partial class AttackCardBack : AttackCardSide
    {

        private TopicName _boundTopic;
        private Godot.Collections.Array<TopicButton> _topicButtonsAttack = new();
        public event TopicHandler TopicPicked;

        [Export]
        public GridContainer TopicGrid
        {
            get; set;
        }
        public TopicName BoundTopic
        {
            get { return _boundTopic; }
            set { _boundTopic = value; }
        }

        public void Setup(PlayerAttack attack)
        {
            _attackName.Text = attack.AttackName;
            _conversationInterestDamage.Text = $"{attack.ConversationInterestDamage}";
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
                    TopicGrid.AddChild(button);
                }
                _attackType.Text = "T";
            }
            else
            {
                _attackType.Text = "S";
            }
            if (attack.OwningCharacter == CharacterName.Marc)
            {
                _coloredCorner.Modulate = _marcColor;
            }
            else
            {
                _coloredCorner.Modulate = _anthonyColor;
            }
        }

        public void ChildPressed(TopicButton topicButton)
        {
            TopicPicked?.Invoke(topicButton.ConversationTopic);
        }
    }
}