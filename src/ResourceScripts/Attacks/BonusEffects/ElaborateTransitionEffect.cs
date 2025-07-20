using Godot;

namespace tee
{
    [GlobalClass]
    public partial class ElaborateTransitionEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager)
        {
            //Ignore the potential Annoyance from topic switch. You still get +1 Annoyance if you switch to a topic they hate
            combatManager.IgnoreTopicSwitchAnnoyance(); 
            combatManager.Enemy.DecreaseEnthusiasmFor(combatManager.Enemy.CurrentTopicName);
            combatManager.Enemy.DecreaseEnthusiasmFor(combatManager.Enemy.CurrentTopicName);
            //TODO: Update _preferenceDisplay for the topic we're transitioning from
            GD.Print("Resolved ElaborateTransitionEffect.");
        }
    }
}
