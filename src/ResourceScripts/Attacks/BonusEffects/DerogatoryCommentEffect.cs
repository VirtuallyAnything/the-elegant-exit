using Godot;

namespace tee
{
    [GlobalClass]
    public partial class DerogatoryCommentEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.IgnoreNextEnthusiasm();
            Preference preference = combatManager.PreferenceForCurrentTopic;
            combatManager.ConversationInterestBonusDamage *= -1;
            switch (preference)
            {
                case Preference.Like:
                    combatManager.Enemy.IncreaseAnnoyance();
                    combatManager.ConversationInterestDamage += 2;
                    break;
                case Preference.Dislike:
                    combatManager.Enemy.DecreaseAnnoyance();
                    break;
            }
            GD.Print("Resolved DerogatoryCommentEffect.");
        }
    }
}
