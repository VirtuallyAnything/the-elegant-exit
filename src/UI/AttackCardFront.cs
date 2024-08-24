using Godot;
using System;

namespace tee
{
    public partial class AttackCardFront : AttackCardSide
    {
        [Export]
        private RichTextLabel _description;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Setup(PlayerAttack attack){
            _attackName.Text = attack.AttackName;
            _conversationInterestDamage.Text = $"{attack.ConversationInterestDamage}";
            _description.Text = attack.BonusEffect?.Description;
        }

    }
}