using Godot;

namespace tee
{
    /// <summary>
    /// Abstract base class for what to display on an Attack Card.
    /// </summary>
    public abstract partial class AttackCardSide : TextureButton
    {
        [Export] protected Label _attackName;
        [Export] protected Label _conversationInterestDamage;
        [Export] protected Label _dynamicCIDamage;
        [Export] protected Label _attackType;
        [Export] protected TextureRect _coloredCorner;
        [Export] protected Color _marcColor;
        [Export] protected Color _anthonyColor;

        public abstract void Setup(PlayerAttack attack);
    }
}