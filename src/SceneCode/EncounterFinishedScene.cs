using Godot;
namespace tee
{
	public partial class EncounterFinishedScene : Node
	{
		[Export] public Label _outcome;
		public void DisplayOutcome(bool hasPlayerWon)
        {
            if (hasPlayerWon)
            {
				_outcome.Text = "You survived the Conversation!";
            }
            else
            {
				_outcome.Text = "This Conversation took its toll...";
            }
        }
	}
}


