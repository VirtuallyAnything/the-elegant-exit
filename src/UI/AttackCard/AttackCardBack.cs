using Godot;
using System;
using System.Collections.Generic;

namespace tee
{
    public delegate void TopicHandler(TopicName topicName);
    public partial class AttackCardBack : AttackCardSide
    {
        private TopicName _boundTopic;
        private Godot.Collections.Array<TopicButton> _topicButtonsAttack = new();
        [Export] private GridContainer _topicGrid;
        private static Dictionary<TopicName, Preference> _discoveredPreferences = new();
        public event TopicHandler TopicPicked;
        public TopicName BoundTopic
        {
            get { return _boundTopic; }
            set { _boundTopic = value; }
        }

        override public void Setup(PlayerAttack attack)
        {
            _attackName.Text = attack.AttackName;
            _conversationInterestDamage.Text = $"{attack.ConversationInterestDamage}";
            if (attack is TopicalPlayerAttack topicalPlayerAttack)
            {
                CombatManager.PreferenceDiscovered += AddPreference;
                TopicButton.OnButtonHovered += OnHover;
                foreach (TopicName conversationTopic in topicalPlayerAttack.UnlockedTopics)
                {
                    TopicButton button = new()
                    {
                        Text = conversationTopic.ToString(),
                        ConversationTopic = conversationTopic,
                        Parent = this
                    };
                    button.MouseExited += EndHover;
                    _topicButtonsAttack.Add(button);
                    _topicGrid.AddChild(button);
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

        private static void AddPreference(TopicName topicName, Preference preference)
        {
            if (!_discoveredPreferences.ContainsKey(topicName))
            {
                _discoveredPreferences.Add(topicName, preference);
            } 
        }

        public void ChildPressed(TopicButton topicButton)
        {
            TopicPicked?.Invoke(topicButton.ConversationTopic);
        }

        public void OnHover(TopicName topicName)
        {
            if (_discoveredPreferences.ContainsKey(topicName))
            {
                int dynamicCiDamage = CombatManager.GetDynamicCIDamageFor(_discoveredPreferences[topicName]);
                if (dynamicCiDamage > 0)
                {
                    _dynamicCIDamage.Text = $"+{dynamicCiDamage}";
                }
                else if (dynamicCiDamage < 0)
                {
                    _dynamicCIDamage.Text = $"{dynamicCiDamage}";
                }
                else
                {
                    _dynamicCIDamage.Text = "";
                }
            }
        }

        public void EndHover()
        {
            _dynamicCIDamage.Text = "";
        }

        public static void ClearPreferences()
        {
            _discoveredPreferences.Clear();
        }

        public override void _ExitTree()
        {
            foreach(TopicButton button in _topicButtonsAttack)
            {
                button.MouseExited -= EndHover;
            }
            CombatManager.PreferenceDiscovered -= AddPreference;
            TopicButton.OnButtonHovered -= OnHover;
        }
    }
}