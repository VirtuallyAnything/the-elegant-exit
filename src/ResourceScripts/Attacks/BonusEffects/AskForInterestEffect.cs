using Godot;

namespace tee
{
    [GlobalClass]
    public abstract partial class AskForInterestEffect : TopicalBonusEffect
    {
        public override void Resolve(CombatManager combatManager, TopicName topicName){
            combatManager.IgnoreCIBonusDamage();
            combatManager.IgnoreNextAnnoyance();
            combatManager.IgnoreNextEnthusiasm();
            combatManager.NextTopicName = topicName;
            Preference preference = combatManager.PreferenceForCurrentTopic;
            if(preference == Preference.Like){
                combatManager.SocialStanding += 1;
            }
        }
    }
}
