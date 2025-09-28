using Godot;

namespace tee
{
    public partial class EncounterTutorial : Control
    {
        private LinkedDialogue _annoyancelDialogue;
        private LinkedDialogue _enthusiasmDialogue;

        public override void _Ready()
        {
            GetTree().Paused = true;
            CombatManager.EnemyTurnComplete += ShowTutorial;
        }

        public void OnPositiveButtonPressed()
        {

        }

        public void OnNegativeButtonPressed()
        {
            Visible = false;
            QueueFree();
        }

        private void ShowTutorial()
        {
            Control tutorial = (Control)ResourceLoader.Load<PackedScene>("res://Subscenes/Tutorial.tscn").Instantiate();
            AddChild(tutorial);
        }

        private void ShowAnnoyanceExplanation()
        {

        }

        private void ShowEnthusiasmExplanation()
        {

        }
        
    }
}