using Godot;

namespace tee
{
    [GlobalClass]
    public partial class TalkShopEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager)
        {
            Preference preference = combatManager.PreferenceForCurrentTopic;
            EncounterEnemy enemy = combatManager.Enemy;
            switch (preference)
            {
                case Preference.Like:
                    enemy.DecreaseAnnoyance();
                    combatManager.SocialStanding += 1;
                    GD.Print($"Resolved TalkShopEffect. Bonus Social Standing +1, now at {combatManager.SocialStanding}");
                    break;
                case Preference.Dislike:
                    enemy.IncreaseAnnoyance();
                    combatManager.ConversationInterestDamage += 2;
                    goto default;  
                default:
                    GD.Print("Resolved TalkShopEffect.");
                    break;
            }
        }
    }
}
