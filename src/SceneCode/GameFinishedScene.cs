using Godot;

namespace tee
{
    public partial class GameFinishedScene : Node
    {
        [Export] private RichTextLabel _finalScore;
        [Export] private TextureRect _outcome;
        [Export] private Texture2D _positiveOutcome;
        [Export] private Texture2D _neutralOutcome;
        [Export] private Texture2D _negativeOutcome;

        public void DisplayScore(int finalScore)
        {
            _finalScore.Text = $"{finalScore}";

            switch (finalScore)
            {
                case < -10:
                    _outcome.Texture = _negativeOutcome;
                    break;
                case > 0:
                    _outcome.Texture = _positiveOutcome;
                    break;
                default:
                    _outcome.Texture = _neutralOutcome;
                    break;
            }
        }
    }
}