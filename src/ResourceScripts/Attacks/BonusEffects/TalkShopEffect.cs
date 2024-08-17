using Godot;

namespace tee
{
    [GlobalClass]
    public abstract partial class TalkShopEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            Preference preference = combatManager.PreferenceForCurrentTopic;
            switch(preference){
                case Preference.Like:
                //combatManager.AnnoyanceLevel.Increase();
                break;
                case Preference.Dislike:
                break;
            }
        }
    }
}
