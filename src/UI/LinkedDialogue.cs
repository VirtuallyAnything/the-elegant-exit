using System.Collections.Generic;
using Godot;

namespace tee
{
    public delegate void LinkedDialogueHandler();
    public partial class LinkedDialogue : Control
    {
        public event LinkedDialogueHandler RequestCloseDialogue;
        [Export] private LinkedDialogue NextDialogue;
        [Export] private LinkedDialogue PrevDialogue;

        public void OnProgressForward()
        {
            if (NextDialogue is not null)
            {
                Visible = false;
                NextDialogue.Visible = true;
            }
        }

        public void OnProgressBackward()
        {
            if (PrevDialogue is not null)
            {
                Visible = false;
                PrevDialogue.Visible = true;
            }
        }

        public void OnRequestCloseDialogue()
        {
            RequestCloseDialogue?.Invoke();
        }

    }
}