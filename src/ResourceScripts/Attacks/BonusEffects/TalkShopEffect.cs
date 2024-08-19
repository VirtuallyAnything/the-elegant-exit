using Godot;

namespace tee
{
    [GlobalClass]
    public abstract partial class TalkShopEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            Preference preference = combatManager.PreferenceForCurrentTopic;
            EncounterEnemy enemy = combatManager.Enemy;
            switch(preference){
                case Preference.Like:
                enemy.DecreaseAnnoyance();
                enemy.IncreaseEnthusiasmFor(combatManager.CurrentTopicName);
                break;
                case Preference.Dislike:
                enemy.IncreaseAnnoyance();
                combatManager.ConversationInterestDamage += 2;
                break;
            }
        }
    }
}
