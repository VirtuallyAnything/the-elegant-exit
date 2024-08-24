using Godot;

namespace tee
{
    [GlobalClass]
    public partial class GiveComplimentEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.Enemy.DecreaseAnnoyance();
            combatManager.SocialStanding++;
        }
    }
}
