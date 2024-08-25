using Godot;
using System;

public delegate void DialogueButtonHandler(VariableDialogueButton variableDialogueButton);
public partial class VariableDialogueButton : Button
{
    public event DialogueButtonHandler DialogueButtonPressed;
    [Export] private NotifyRichText _RTlabel;
    public NotifyRichText RTLabel{
        get{return _RTlabel;}
    }
    new public string Text
    {
        get { return _RTlabel.Text; }
        set { _RTlabel.Text = value; }
    }

    public override void _Ready()
    {
        if(_RTlabel == null){
            _RTlabel = new();
            AddChild(_RTlabel);
        }
        _RTlabel.SizePropertyChanged += SetToLabelSize;
        Pressed += OnButtonPressed;
    }

    private void SetToLabelSize()
    {
        Vector2 difference = _RTlabel.Position - Position;
        Size = _RTlabel.Size + new Vector2(1.5f, 2) * difference.Abs();
    }

    private void OnButtonPressed(){
        DialogueButtonPressed?.Invoke(this);
    }

    public void SetEntireVisibility(bool visible){
        _RTlabel.Visible = visible;
        Visible = visible;
    }
}
