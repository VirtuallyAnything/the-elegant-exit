using Godot;
using System;

public partial class EnthusiasmLevel : GodotObject
{
    private bool _levelThreeReached;
    private bool _levelFiveReached;
    private int _currentEnthusiasm;
    private float _socialStandingChange;
    private int _conversationInterestModifier;
    private int _mentalCapacityBonus;
    private int _annoyance;

    public void Increase(){
        if(_currentEnthusiasm == 5){
            return;
        }
        _currentEnthusiasm++;
        switch(_currentEnthusiasm){
            case 2: 
            _socialStandingChange = 1;
            _conversationInterestModifier += 2;
            break;
            case 3: 
            _socialStandingChange = 2;
            _conversationInterestModifier += 1;
            if(!_levelThreeReached){
                _annoyance -= 1;
                _levelThreeReached = true;
            }
            break;
            case 4: 
            _socialStandingChange = 4;
            _conversationInterestModifier += 1; 
            _mentalCapacityBonus += 1;
            break;
            case 5: 
            _socialStandingChange = 5;
            _conversationInterestModifier += 1;
            if(!_levelFiveReached){
                _annoyance -= 1;
                _levelFiveReached = true;
            }
            break; 
        }
    }

    public void Decrease(){
        if(_currentEnthusiasm == 0){
            return;
        }
        _currentEnthusiasm--;
        switch(_currentEnthusiasm){
            case 1:
            _socialStandingChange = 0;
            _conversationInterestModifier = 0;
            break;
            case 2: 
            _socialStandingChange = 1;
            _conversationInterestModifier -= 1;
            break;
            case 3: 
            _socialStandingChange = 2;
            _conversationInterestModifier -= 1;
            _mentalCapacityBonus = 0;
            break;
            case 4: 
            _socialStandingChange = 4;
            _conversationInterestModifier -= 1; 
            break;
        }
    }
}
