using Godot;

namespace tee
{
    public abstract partial class BonusEffect : Resource
    {
        public abstract void Resolve(CombatManager combatManager);
    }
}
