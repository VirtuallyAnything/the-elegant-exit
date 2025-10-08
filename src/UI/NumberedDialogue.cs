using Godot;

namespace tee
{
    public partial class NumberedDialogue : Control
    {
        [Export] private Label _numberLabel;

        public void SetNumber(int currentNumber, int maxNumber)
        {
            _numberLabel.Text = $"{currentNumber} / {maxNumber}";
        }
    }
}
