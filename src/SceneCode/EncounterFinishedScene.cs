using Godot;
namespace tee
{
   public partial class EncounterFinishedScene : Scene
   {
      [Export] public Label _outcome;
      public void DisplayOutcome(EncounterOutcome outcome)
      {
         switch (outcome)
         {
            case EncounterOutcome.PlayerDefeated:
               _outcome.Text = "This Conversation took its toll...";
               // -value[Social Battery Icon]
               break;
            case EncounterOutcome.EnemyDefeated:
               _outcome.Text = "You survived the Conversation!";
               // Remaining [Mental Capacity Icon]: value
               break;
            case EncounterOutcome.MaxAnnoyanceReached:
               _outcome.Text = "Your conversation partner was fed up with you and left.";
               // -?[Social Standing Icon]
               break;
         }
      }
   }
}


