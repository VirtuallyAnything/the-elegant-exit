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

    }
}