using Godot;
using System;

namespace tee
{
    public partial class AttackCardFront : AttackCardSide
    {
        private Label _attackName;
        private Label _conversationInterestDamage;
        private RichTextLabel _description;

        public AttackCardFront(PlayerAttack attack){
            _attackName.Text = attack.AttackName;
            _conversationInterestDamage.Text = $"{attack.ConversationInterestDamage}";
            _description.Text = attack.BonusEffect?.Description;
        }
    }
}