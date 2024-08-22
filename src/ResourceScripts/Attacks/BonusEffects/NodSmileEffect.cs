using Godot;

namespace tee
{
    [GlobalClass]
    public partial class NodSmileEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.BlockNextEnemyAttack();
        }
    }
}
