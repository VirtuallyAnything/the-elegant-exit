using Godot;
using System;

namespace tee
{
    public delegate void EnthusiasmHandler();
    public struct EnthusiasmData
    {
        public int CurrentEnthusiasm;
        public int ConversationInterestModDelta;
        public int SocialStandingChange;
    }
    public partial class EnthusiasmLevel : GodotObject
    {
        public static event EnthusiasmHandler AnnoyanceLowered;
        private bool _levelThreeReached;
        private bool _levelFiveReached;
        private int _currentEnthusiasm;
        private int _socialStandingChange;
        private int _conversationInterestModTotal;
        private int _conversationInterestModDelta;
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
            get { return _conversationInterestModTotal; }
        }

        public EnthusiasmData PackageCurrentData()
        {
            return new EnthusiasmData()
            {
                CurrentEnthusiasm = CurrentEnthusiasm,
                ConversationInterestModDelta = _conversationInterestModDelta,
                SocialStandingChange = _socialStandingChange
            };
        }

        public void Increase()
        {
            if (_currentEnthusiasm == 5)
            {
                _conversationInterestModDelta = 0;
                return;
            }
            _currentEnthusiasm++;
            int prevCoversationInterestMod = _conversationInterestModTotal;
            switch (CurrentEnthusiasm)
            {
                case 2:
                    _socialStandingChange = 1;
                    _conversationInterestModTotal += 2;
                    break;
                case 3:
                    _socialStandingChange = 2;
                    _conversationInterestModTotal += 1;
                    if (!_levelThreeReached)
                    {
                        AnnoyanceLowered?.Invoke();
                        _levelThreeReached = true;
                    }
                    break;
                case 4:
                    _socialStandingChange = 4;
                    _conversationInterestModTotal += 1;
                    _enemyMentalCapacityBonusDamage += 1;
                    break;
                case 5:
                    _socialStandingChange = 5;
                    _conversationInterestModTotal += 1;
                    if (!_levelFiveReached)
                    {
                        AnnoyanceLowered?.Invoke();
                        _levelFiveReached = true;
                    }
                    break;
            }
            _conversationInterestModDelta = _conversationInterestModTotal - prevCoversationInterestMod;
        }

        public void Decrease()
        {
            if (CurrentEnthusiasm == 0)
            {
                _conversationInterestModDelta = 0;
                return;
            }
            _currentEnthusiasm--;
            int prevCoversationInterestMod = _conversationInterestModTotal;
            switch (CurrentEnthusiasm)
            {
                case 1:
                    _socialStandingChange = 0;
                    _conversationInterestModTotal -= 2;
                    break;
                case 2:
                    _socialStandingChange = 1;
                    _conversationInterestModTotal -= 1;
                    break;
                case 3:
                    _socialStandingChange = 2;
                    _conversationInterestModTotal -= 1;
                    _enemyMentalCapacityBonusDamage = 0;
                    break;
                case 4:
                    _socialStandingChange = 4;
                    _conversationInterestModTotal -= 1;
                    break;
            }
            _conversationInterestModDelta = _conversationInterestModTotal - prevCoversationInterestMod;
        }
    }
}
