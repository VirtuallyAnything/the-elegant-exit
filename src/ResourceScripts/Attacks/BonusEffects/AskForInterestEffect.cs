using Godot;

namespace tee
{
    [GlobalClass]
    public partial class AskForInterestEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.IgnoreCIBonusDamage();
            combatManager.IgnoreNextAnnoyance();
            combatManager.IgnoreNextEnthusiasm();
            combatManager.NextTopicName = combatManager.PlayerCurrentTopicName;
            Preference preference = combatManager.PreferenceForCurrentTopic;
            if(preference == Preference.Like){
                combatManager.SocialStanding += 1;
            }
        }
    }
}
