using Godot;

namespace tee
{
    [GlobalClass]
    public abstract partial class AskForInterestEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.BlockNextEnemyAttack();
        }
    }
}
