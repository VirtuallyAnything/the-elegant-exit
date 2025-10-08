using Godot;

namespace tee
{
    public delegate void LinkedDialogueHandler();
    public partial class LinkedDialogueController : Control
    {
        public event LinkedDialogueHandler RequestClose;
        [Export] private Godot.Collections.Array<NumberedDialogue> _linkedDialogues;
        private int _currentDialogueIndex;

        public override void _Ready()
        {
            _currentDialogueIndex = 0;
            InitiateCurrentDialogue();
        }

        private void InitiateCurrentDialogue()
        {
            NumberedDialogue currentDialogue = _linkedDialogues[_currentDialogueIndex];
            currentDialogue.Visible = true;
            currentDialogue.SetNumber(_currentDialogueIndex + 1, _linkedDialogues.Count);
        }

        public void OnProgressForward()
        {
            _linkedDialogues[_currentDialogueIndex].Visible = false;
            _currentDialogueIndex++;
            if (_currentDialogueIndex < _linkedDialogues.Count)
            {
                InitiateCurrentDialogue();
            }
            else
            {
                Visible = false;
                RequestClose?.Invoke();
            }
        }

        public void OnProgressBackward()
        {
            _linkedDialogues[_currentDialogueIndex].Visible = false;
            _currentDialogueIndex--;
            if (_currentDialogueIndex >= 0)
            {
                InitiateCurrentDialogue();
            }
            else
            {
                _currentDialogueIndex = 0;
            }
        }
    }
}