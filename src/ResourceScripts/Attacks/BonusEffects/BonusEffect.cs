using Godot;

namespace tee
{
    public abstract partial class BonusEffect : Resource
    {
        [Export]
        private string _description;
        public string Description
        {
            get { return _description; }
        }

        public abstract void Resolve(CombatManager combatManager);
    }
}
