using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace tee
{
    public partial class EncounterPlayer : GodotObject
    {
        private float _mentalCapacity = 10;
        private Array<PlayerAttack> _allPlayerAttacks;
        private Array<PlayerAttack> _attackPool;
        private Array<PlayerAttack> _currentAttacks = new();
        private System.Collections.Generic.Dictionary<TopicName, Preference> _discoveredEnemyPreferences = new();
        public System.Collections.Generic.Dictionary<TopicName, Preference> DiscoveredEnemyPreferences
        {
            get { return _discoveredEnemyPreferences; }
        }
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
        public EncounterPlayer(Array<PlayerAttack> playerAttacks)
        {
            _allPlayerAttacks = playerAttacks;
            _attackPool = new Array<PlayerAttack>(_allPlayerAttacks);
            CombatManager.PreferenceDiscovered += AddEnemyPreference;
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

        public PlayerAttack SwapAttackOut(PlayerAttack attack)
        {
            _currentAttacks.Remove(attack);
            PlayerAttack newAttack = ChooseRandomAttack();
            _currentAttacks.Add(newAttack);
            return newAttack;
        }

        public void AddEnemyPreference(TopicName topicName, Preference preference)
        {
            if (!DiscoveredEnemyPreferences.ContainsKey(topicName))
            {
                DiscoveredEnemyPreferences.Add(topicName, preference);
            }
        }

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
            {
                CombatManager.PreferenceDiscovered -= AddEnemyPreference;
            }
        }
    }
}
