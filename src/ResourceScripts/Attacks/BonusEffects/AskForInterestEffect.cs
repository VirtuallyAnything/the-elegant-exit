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
            combatManager.NextTopicName = combatManager.Player.CurrentTopicName;
            Preference preference = combatManager.PreferenceForCurrentTopic;
            if(preference == Preference.Like){
                combatManager.SocialStanding += 1;
                GD.Print($"Resolved AskForInterestEffect. Bonus Social Standing +1, now at {combatManager.SocialStanding}");
            }
            else
            {
                GD.Print("Resolved AskForInterestEffect.");
            }  
        }
    }
}
