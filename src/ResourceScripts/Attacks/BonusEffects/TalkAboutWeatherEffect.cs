using Godot;

namespace tee
{
    [GlobalClass]
    public partial class TalkAboutWeatherEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            combatManager.PlayerCurrentTopicName = TopicName.Weather;
            GD.Print("Resolved TalkAboutWeatherEffect.");
        }
    }
}
