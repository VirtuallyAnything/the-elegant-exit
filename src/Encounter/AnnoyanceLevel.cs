using Godot;
using System;

namespace tee
{
    public delegate void AnnoyanceHandlerObject(AnnoyanceData annoyanceData);
    public delegate void AnnoyanceHandlerValue(int value);
    public delegate void AnnoyanceHandler();
    public struct AnnoyanceData
    {
        public int ConversationInterestModifier;
        public int SocialStandingChange;
        public int SocialBatteryDamage;
        public int CurrentAnnoyance;
    }
    /// <summary>
    /// An object to keep track of an Enemy's annoyance.
    /// </summary>
    public partial class AnnoyanceLevel : GodotObject
    {
        /// <summary>
        /// Invoked when CurrentAnnoyance increases or decreases. Sends out an AnnoyanceData struct containing all changes for the new level of Annoyance.
        /// </summary>
        public static event AnnoyanceHandlerObject Changed;
        private static int _maxAnnoyance = 5;
        private int _currentAnnoyance;
        private int _socialStandingChange;
        private int _conversationInterestModifier;
        private int _socialBatteryDamage;
        public int CurrentAnnoyance
        {
            get { return _currentAnnoyance; }
            set
            {
                if (value > MaxAnnoyance)
                {
                    _currentAnnoyance = MaxAnnoyance;
                }
                else if (value < 0)
                {
                    _currentAnnoyance = 0;
                }
                else
                {
                    _currentAnnoyance = value;
                }
            }
        }
        public static int MaxAnnoyance
        {
            get { return _maxAnnoyance; }
        }
        public int SocialStandingChange
        {
            get { return _socialStandingChange; }
        }
        public int ConversationInterestModifier
        {
            get { return _conversationInterestModifier; }
        }

        /// <summary>
        /// Increases CurrentAnnoyance by 1.
        /// </summary>
        public void Increase()
        {
            if (CurrentAnnoyance == MaxAnnoyance)
            {
                GD.Print("Highest Annoyance Level already reached.");
                return;
            }
            CurrentAnnoyance++;

            switch (CurrentAnnoyance)
            {
                case 1:
                    _socialStandingChange = -1;
                    _conversationInterestModifier = -1;
                    _socialBatteryDamage = -1;
                    break;
                case 2:
                    _socialStandingChange = -3;
                    _conversationInterestModifier = -2;
                    _socialBatteryDamage = -1;
                    break;
                case 3:
                    _socialStandingChange = -6;
                    _conversationInterestModifier = -3;
                    _socialBatteryDamage = -2;
                    break;
                case 4:
                    _socialStandingChange = -10;
                    _conversationInterestModifier = -4;
                    _socialBatteryDamage = -2;
                    break;
                case 5:
                    _socialStandingChange = -15;
                    break;
            }
            Changed?.Invoke(new AnnoyanceData()
            {
                ConversationInterestModifier = _conversationInterestModifier,
                SocialStandingChange = _socialStandingChange,
                SocialBatteryDamage = _socialBatteryDamage,
                CurrentAnnoyance = CurrentAnnoyance
            });
            GD.Print($"Annoyance Level increased to {CurrentAnnoyance}");
        }

        /// <summary>
        /// Increases CurrentAnnoyance.
        /// </summary>
        /// <param name="amount">Number of steps to increase.</param>
        public void Increase(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Increase();
            }
        }

        /// <summary>
        /// Decreases CurrentAnnoyance by 1.
        /// </summary>
        public void Decrease()
        {
            if (CurrentAnnoyance == 0)
            {
                GD.Print("Lowest Annoyance Level already reached.");
                return;
            }
            CurrentAnnoyance--;

            switch (CurrentAnnoyance)
            {
                case 0:
                    _socialStandingChange = 0;
                    _conversationInterestModifier = 1;
                    break;
                case 1:
                    _socialStandingChange = -1;
                    _conversationInterestModifier = 2;
                    break;
                case 2:
                    _socialStandingChange = -3;
                    _conversationInterestModifier = 3;
                    break;
                case 3:
                    _socialStandingChange = -6;
                    _conversationInterestModifier = 4;
                    break;
            }
            Changed?.Invoke(new AnnoyanceData()
            {
                ConversationInterestModifier = _conversationInterestModifier,
                SocialStandingChange = _socialStandingChange,
                SocialBatteryDamage = _socialBatteryDamage,
                CurrentAnnoyance = CurrentAnnoyance
            });
            GD.Print($"Annoyance Level decreased to {CurrentAnnoyance}");
        }

        /// <summary>
        /// Decreases CurrentAnnoyance.
        /// </summary>
        /// <param name="amount">Number of steps to decrease.</param>
        public void Decrease(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Decrease();
            }
        }

        /// <summary>
        /// Helper function for UI purposes.
        /// </summary>
        /// <param name="level">The level for which to retrieve the value of social battery damage</param>
        /// <returns>Social battery damage independent of CurrentAnnoyance.</returns>
        public static int GetSocialBatteryDamageForLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return -1;
                case 2:
                    return -1;
                case 3:
                    return -2;
                case 4:
                    return -2;
            }
            return 0;
        }

        /// <summary>
        /// Helper function for UI purposes.
        /// </summary>
        /// <param name="level">The level for which to retrieve the delta value of conversation interest damage</param>
        /// <returns>Delta conversation interest if one were to Increase Annoyance to level.</returns>
        public static int GetCIDeltaForIncreaseTo(int level)
        {
            switch (level)
            {
                case 1:
                    return -1;
                case 2:
                    return -2;
                case 3:
                    return -3;
                case 4:
                    return -4;
            }
            return 0;
        }

        /// <summary>
        /// Helper function for UI purposes.
        /// </summary>
        /// <param name="level">The level for which to retrieve the delta value of conversation interest damage</param>
        /// <returns>Delta conversation interest if one were to Decrease Annoyance to level.</returns>
        public static int GetCIDeltaForDecreaseTo(int level)
        {
            switch (level)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
            }
            return 0;
        }
    }
}
