using Godot;

namespace tee
{
    [GlobalClass]
    public partial class ShowDisinterestEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            EncounterEnemy enemy = combatManager.Enemy;
            enemy.SwitchTopicNextTurn();
            enemy.DecreaseEnthusiasmFor(enemy.CurrentTopicName);
            GD.Print($"Resolved ShowDisinterestEffect. Disinterest shown for {enemy.CurrentTopicName}");
        }
    }
}
