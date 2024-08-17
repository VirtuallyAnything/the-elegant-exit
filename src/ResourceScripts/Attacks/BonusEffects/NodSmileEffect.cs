using Godot;

namespace tee
{
    [GlobalClass]
    public abstract partial class NodSmileEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.BlockNextEnemyAttack();
        }
    }
}
