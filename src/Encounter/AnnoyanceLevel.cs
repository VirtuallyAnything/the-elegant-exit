using Godot;
using System;

namespace tee
{
    public delegate void AnnoyanceHandlerArg(int value, GodotObject godotObject);
    public delegate void AnnoyanceHandler();
    
    public partial class AnnoyanceLevel : GodotObject
    {
        public event AnnoyanceHandlerArg SocialBatteryChanged;
        public static event AnnoyanceHandlerArg ConversationInterestChanged;
        public event AnnoyanceHandler LevelFiveReached;
        private int _currentAnnoyance;
        private int _socialStandingChange;
        private int _conversationInterestModifier;
        private int _socialBatteryDamage;
        public int CurrentAnnoyance
        {
            get { return _currentAnnoyance; }
            set
            {
                if (value > 5)
                {
                    _currentAnnoyance = 5;
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
        public int SocialStandingChange
        {
            get { return _socialStandingChange; }
        }
        public int ConversationInterestModifier
        {
            get { return _conversationInterestModifier; }
        }

        public void Increase()
        {
            if (CurrentAnnoyance == 5)
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
                    LevelFiveReached?.Invoke();
                    break;
            }
            SocialBatteryChanged?.Invoke(_socialBatteryDamage, this);
            ConversationInterestChanged?.Invoke(_conversationInterestModifier, this);
            GD.Print($"Annoyance Level increased to {CurrentAnnoyance}");
        }

        public void Increase(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Increase();
            }
        }

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
            ConversationInterestChanged?.Invoke(_conversationInterestModifier, this);
            GD.Print($"Annoyance Level decreased to {CurrentAnnoyance}");
        }


        public void Decrease(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Decrease();
            }
        }

       public int GetTotalSocialStanding(){
            return _socialStandingChange;
       }

        public int GetSocialBatteryDamageForLevel(int level)
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

        public int GetCIDeltaForIncreaseTo(int level){
            switch(level){
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

        public int GetCIDeltaForDecreaseTo(int level){
            switch(level){
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
