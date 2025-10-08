using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace tee
{
    public partial class EncounterTutorial : Control
    {
        [Export] private PackedScene _overviewDialogue;
        [Export] private PackedScene _topicalAttacksDialogue;
        [Export] private PackedScene _annoyanceDialogue;
        [Export] private PackedScene _enthusiasmDialogue;
        [Export] private Control _confirmationWindow;
        private LinkedDialogueController _currentDialogue;
        private List<PackedScene> _queuedTutorials = new();

        public override void _Ready()
        {
            GetTree().Paused = true;
        }

        public void OnPositiveButtonPressed()
        {
            AnnoyanceLevel.Changed += AddTutorial;
            ConversationTopic.EnthusiasmChangedForTopic += AddTutorial;
            CombatManager.TopicalAttackActive += AddTutorial;

            _queuedTutorials.Add(_overviewDialogue);
            _confirmationWindow.Visible = false;
            _confirmationWindow.QueueFree();
            GetTree().Paused = false;
        }

        public void OnNegativeButtonPressed()
        {
            Visible = false;
            GetTree().Paused = false;
            QueueFree();
        }

        private void DisplayTutorial(PackedScene tutorial)
        {
            try
            {
                _currentDialogue = (LinkedDialogueController)tutorial.Instantiate();
                AddChild(_currentDialogue);
                _currentDialogue.RequestClose += CloseTutorial;
            }
            catch (InvalidCastException)
            {
                GD.PrintErr("Instantiated Tutorial does not have a base node of type LinkedDialogueController.");
            }
            GetTree().Paused = true;
            Visible = true;
        }

        private PackedScene DecideNextTutorial(PackedScene lastAddedTutorial)
        {
            PackedScene nextTutorial;

            if (_queuedTutorials.Count > 0)
            {
                nextTutorial = _queuedTutorials.First();
                _queuedTutorials.Remove(nextTutorial);
                _queuedTutorials.Add(lastAddedTutorial);
            }
            else
            {
                nextTutorial = lastAddedTutorial;
            }

            return nextTutorial;
        }

        private void AddTutorial()
        {
            PackedScene nextTutorial = DecideNextTutorial(_topicalAttacksDialogue);
            if (_currentDialogue is null)
            {
                DisplayTutorial(nextTutorial);
            }
            CombatManager.TopicalAttackActive -= AddTutorial;
        }

        private void AddTutorial(AnnoyanceData annoyanceData)
        {
            PackedScene nextTutorial = DecideNextTutorial(_annoyanceDialogue);
            if (_currentDialogue is null)
            {
                DisplayTutorial(nextTutorial);
            }
            AnnoyanceLevel.Changed -= AddTutorial;
        }

        private void AddTutorial(EnthusiasmData enthusiasmData, TopicName topicName)
        {
            PackedScene nextTutorial = DecideNextTutorial(_enthusiasmDialogue);
            if (_currentDialogue is null)
            {
                DisplayTutorial(nextTutorial);
            }
            ConversationTopic.EnthusiasmChangedForTopic -= AddTutorial;
        }

        private void CloseTutorial()
        {
            _currentDialogue.RequestClose -= CloseTutorial;
            _currentDialogue.QueueFree();
            _currentDialogue = null;

            if (_queuedTutorials.Count > 0)
            {
                PackedScene nextTutorial = _queuedTutorials.First();
                DisplayTutorial(nextTutorial);
                _queuedTutorials.Remove(nextTutorial);
            }
            else
            {
                GetTree().Paused = false;
                Visible = false;
            }
        }

        public override void _ExitTree()
        {
            try
            {
                CombatManager.TopicalAttackActive -= AddTutorial;
                AnnoyanceLevel.Changed -= AddTutorial;
                ConversationTopic.EnthusiasmChangedForTopic -= AddTutorial;
            }
            catch { }
        }

    }
}