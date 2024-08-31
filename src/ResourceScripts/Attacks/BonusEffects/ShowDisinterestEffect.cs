using Godot;

namespace tee
{
    [GlobalClass]
    public partial class ShowDisinterestEffect : BonusEffect
    {
        public override void Resolve(CombatManager combatManager){
            EncounterEnemy enemy = combatManager.Enemy;
            enemy.SwitchTopicNextTurn();
            enemy.DecreaseEnthusiasmFor(combatManager.Enemy.CurrentTopicName);
            GD.Print($"Resolved ShowDisinterestEffect. Disinterest shown for {combatManager.Enemy.CurrentTopicName}");
        }
    }
}
