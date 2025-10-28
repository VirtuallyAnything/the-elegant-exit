using Godot;

namespace tee
{
    [GlobalClass]
    public partial class DerogatoryCommentEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.IgnoreNextEnthusiasm();
            combatManager.IgnoreNextAnnoyance();
            Preference preference = combatManager.PreferenceForCurrentTopic;
            combatManager.IgnoreCIBonusDamage();
            switch (preference)
            {
                case Preference.Like:
                    combatManager.Enemy.IncreaseAnnoyance();
                    combatManager.ConversationInterestBonusDamage = 2;
                    break;
                case Preference.Dislike:
                    combatManager.Enemy.DecreaseAnnoyance();
                    break;
            }
            GD.Print("Resolved DerogatoryCommentEffect.");
        }
    }
}
