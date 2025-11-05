using Godot;
namespace tee
{
   public partial class EncounterFinishedScene : Scene
   {
      [Export] private Label _flavourText;
      [Export] private RichTextLabel _outcome;
      public void DisplayOutcome(EncounterOutcome outcome, int socialStanding, int socialBattery)
      {
         switch (outcome)
         {
            case EncounterOutcome.PlayerDefeated:
               _flavourText.Text = "This Conversation took its toll...";
               _outcome.Text = $"{socialBattery} [img]res://Assets/UI/Icons/SocialBatteryIcon_2.png[/img]";
               break;
            case EncounterOutcome.EnemyDefeated:
               _flavourText.Text = "You survived the Conversation!";
               _outcome.Text = $"Remaining [img]res://Assets/UI/Icons/MentalCapacityIcon.png[/img]: {socialBattery}";
               break;
            case EncounterOutcome.MaxAnnoyanceReached:
               _flavourText.Text = "Your conversation partner was fed up with you and left.";
               _outcome.Text = "-? [img]res://Assets/UI/Icons/SocialStandingIcon.png[/img]";
               break;
         }
      }
   }
}


