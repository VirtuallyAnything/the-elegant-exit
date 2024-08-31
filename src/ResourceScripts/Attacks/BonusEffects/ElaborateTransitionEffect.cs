using Godot;

namespace tee
{
    [GlobalClass]
    public partial class ElaborateTransitionEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager)
        {
            combatManager.IgnoreNextAnnoyance();
            combatManager.Enemy.DecreaseEnthusiasmFor(combatManager.PlayerLastTopicName);
            combatManager.Enemy.DecreaseEnthusiasmFor(combatManager.PlayerLastTopicName);
            GD.Print("Resolved ElaborateTransitionEffect.");
        }
    }
}
