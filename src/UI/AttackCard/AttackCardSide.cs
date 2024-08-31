using Godot;
using System;

namespace tee
{
    public abstract partial class AttackCardSide : TextureButton
    {
        [Export]
        protected Label _attackName;
        [Export]
        protected Label _conversationInterestDamage;
        [Export]
        protected Label _attackType;
        [Export]
        protected TextureRect _coloredCorner;
        [Export]
        protected Color _marcColor;
        [Export]
        protected Color _anthonyColor;

    }
}