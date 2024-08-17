using Godot;
using Godot.Collections;

namespace tee
{
    public partial class EncounterPlayer : GodotObject
    {
        private float _mentalCapacity = 10;
        public float MentalCapacity
        {
            get { return _mentalCapacity; }
            set
            {
                if (value < 0)
                {
                    _mentalCapacity = 0;
                }
                else if (value > 10)
                {
                    _mentalCapacity = 10;
                }
                else
                {
                    _mentalCapacity = value;
                }
            }
        }
        private Array<PlayerAttack> _allPlayerAttacks;
        private Array<PlayerAttack> _attackPool;
        private Array<PlayerAttack> _currentAttacks = new();

        public EncounterPlayer(Array<PlayerAttack> playerAttacks)
        {
            _allPlayerAttacks = playerAttacks;
            _attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
        }
        public PlayerAttack ChooseRandomAttack()
        {
            if (_attackPool.Count == 0)
            {
                _attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
                foreach (PlayerAttack attack in _currentAttacks)
                {
                    _attackPool.Remove(attack);
                }
            }
            PlayerAttack randomAttack = _attackPool.PickRandom();
            _attackPool.Remove(randomAttack);
            _currentAttacks.Add(randomAttack);
            return randomAttack;
        }

        public PlayerAttack SwapAttackOut(PlayerAttack attack){
            _currentAttacks.Remove(attack);
			PlayerAttack newAttack = ChooseRandomAttack();
			_currentAttacks.Add(newAttack);
            return newAttack;
        }
    }
}
