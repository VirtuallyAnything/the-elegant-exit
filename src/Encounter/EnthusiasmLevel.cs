using Godot;
using System;

namespace tee
{
    public delegate void EnthusiasmHandler();
    public delegate void EnthusiasmHandlerObject(EnthusiasmData enthusiasmData);
    public struct EnthusiasmData
    {
        public int ConversationInterestModifier;
        public int SocialStandingChange;
        public int EnemyMentalCapacityBonusDamage;
    }
    public partial class EnthusiasmLevel : GodotObject
    {
        public static event EnthusiasmHandler AnnoyanceLowered;
        public static event EnthusiasmHandlerObject Changed;
        private bool _levelThreeReached;
        private bool _levelFiveReached;
        private int _currentEnthusiasm;
        private int _socialStandingChange;
        private int _conversationInterestModifier;
        private int _conversationInterestModifierTotal;
        private int _enemyMentalCapacityBonusDamage;
        public int CurrentEnthusiasm
        {
            get { return _currentEnthusiasm; }
        }
        public int SocialStandingChange
        {
            get { return _socialStandingChange; }
        }
        public int ConversationInterestModifierTotal
        {
            get { return _conversationInterestModifierTotal; }
        }

        public void Increase()
        {
            if (_currentEnthusiasm == 5)
            {
                return;
            }
            _currentEnthusiasm++;
            switch (_currentEnthusiasm)
            {
                case 2:
                    _socialStandingChange = 1;
                    _conversationInterestModifier = 2;
                    _conversationInterestModifierTotal += 2;
                    break;
                case 3:
                    _socialStandingChange = 2;
                    _conversationInterestModifier = 1;
                    _conversationInterestModifierTotal += 1;
                    if (!_levelThreeReached)
                    {
                        AnnoyanceLowered?.Invoke();
                        _levelThreeReached = true;
                    }
                    break;
                case 4:
                    _socialStandingChange = 4;
                    _conversationInterestModifier = 1;
                    _conversationInterestModifierTotal += 1;
                    _enemyMentalCapacityBonusDamage += 1;
                    break;
                case 5:
                    _socialStandingChange = 5;
                    _conversationInterestModifier = 1;
                    _conversationInterestModifierTotal += 1;
                    if (!_levelFiveReached)
                    {
                        AnnoyanceLowered?.Invoke();
                        _levelFiveReached = true;
                    }
                    break;
            }
            Changed?.Invoke(new EnthusiasmData()
            {
                ConversationInterestModifier = _conversationInterestModifier,
                SocialStandingChange = _socialStandingChange,
                EnemyMentalCapacityBonusDamage = _enemyMentalCapacityBonusDamage
            });
        }

        public void Decrease()
        {
            if (_currentEnthusiasm == 0)
            {
                return;
            }
            _currentEnthusiasm--;
            switch (_currentEnthusiasm)
            {
                case 1:
                    _socialStandingChange = 0;
                    _conversationInterestModifier = -2;
                    _conversationInterestModifierTotal -= 2;
                    break;
                case 2:
                    _socialStandingChange = 1;
                    _conversationInterestModifier = -1;
                    _conversationInterestModifierTotal -= 1;
                    break;
                case 3:
                    _socialStandingChange = 2;
                    _conversationInterestModifier = -1;
                    _conversationInterestModifierTotal -= 1;
                    _enemyMentalCapacityBonusDamage = 0;
                    break;
                case 4:
                    _socialStandingChange = 4;
                    _conversationInterestModifier = -1;
                    _conversationInterestModifierTotal -= 1;
                    break;
            }
            Changed?.Invoke(new EnthusiasmData()
            {
                ConversationInterestModifier = _conversationInterestModifier,
                SocialStandingChange = _socialStandingChange,
                EnemyMentalCapacityBonusDamage = _enemyMentalCapacityBonusDamage
            });
        }
    }
}
