using Godot;

namespace tee
{
    public abstract partial class TopicalBonusEffect : Resource
    {
        public abstract void Resolve(CombatManager combatManager, TopicName topicName);
    }
}
