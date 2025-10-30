using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace tee
{
    public partial class EncounterPlayer : EncounterCharacter
    {
        private int _mentalCapacity;
        private int _maxMentalCapacity = 10;
        private List<PlayerAttack> _allPlayerAttacks;
        private List<PlayerAttack> _attackPool;
        private List<PlayerAttack> _currentAttacks = new();
        private System.Collections.Generic.Dictionary<TopicName, Preference> _discoveredEnemyPreferences = new();
        public System.Collections.Generic.Dictionary<TopicName, Preference> DiscoveredEnemyPreferences
        {
            get { return _discoveredEnemyPreferences; }
        }
        public int MentalCapacity
        {
            get { return _mentalCapacity; }
            set
            {
                if (value < 0)
                {
                    _mentalCapacity = 0;
                }
                else if (value > _maxMentalCapacity)
                {
                    _mentalCapacity = _maxMentalCapacity;
                }
                else
                {
                    _mentalCapacity = value;
                }
            }
        }
        public int MaxMentalCapacity
        {
            get { return _maxMentalCapacity; }
        }
        
        public EncounterPlayer(List<PlayerAttack> playerAttacks)
        {
            _allPlayerAttacks = playerAttacks;
            _attackPool = new(_allPlayerAttacks);
            CombatManager.PreferenceDiscovered += AddEnemyPreference;
        }

        public PlayerAttack ChooseRandomAttack()
        {
            if (_attackPool.Count == 0)
            {
                _attackPool = new List<PlayerAttack>(_allPlayerAttacks);
                foreach (PlayerAttack attack in _currentAttacks)
                {
                    _attackPool.Remove(attack);
                }
            }
            var index = new Random().Next(_attackPool.Count);
            PlayerAttack randomAttack = _attackPool[index];
            _attackPool.Remove(randomAttack);
            _currentAttacks.Add(randomAttack);
            return randomAttack;
        }

        public PlayerAttack SwapAttackOut(PlayerAttack attack)
        {
            _currentAttacks.Remove(attack);
            PlayerAttack newAttack = ChooseRandomAttack();
            return newAttack;
        }

        public void UpdateCurrentAttacks(CombatManager combatManager)
        {
            foreach(PlayerAttack attack in _currentAttacks)
            {
                if(attack is TopicalPlayerAttack topicalAttack)
                {
                    topicalAttack.UpdateAvailableTopics(combatManager);
                }
            }
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
