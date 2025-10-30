using System.Collections.Generic;
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
            GD.Print("Resolved ElaborateTransitionEffect.");
        }

        public override List<TopicName> GetValidTopics(CombatManager combatManager, ref List<TopicName> currentTopics)
        {
            TopicName activeTopic = combatManager.Enemy.CurrentTopicName;
            if (currentTopics.Contains(activeTopic))
            {
                currentTopics.Remove(activeTopic);
            }
            return currentTopics;
        }

    }
}
