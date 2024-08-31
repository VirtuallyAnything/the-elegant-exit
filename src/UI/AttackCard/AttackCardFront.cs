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
            if(attack is TopicalPlayerAttack){
                _attackType.Text = "T";
            }else{
                _attackType.Text = "S";
            }
            if(attack.OwningCharacter == CharacterName.Marc){
                _coloredCorner.Modulate = _marcColor;
            }else{
                _coloredCorner.Modulate = _anthonyColor;
            }
            
        }

    }
}