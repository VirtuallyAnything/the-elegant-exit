using Godot;
using System;

public partial class VariableDialogueControl : Control
{
    [Export] private NotifyRichText _RTlabel;
    public NotifyRichText RTLabel{
        get{return _RTlabel;}
    }
    public string Text
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
    }

    private void SetToLabelSize()
    {
        Vector2 difference = _RTlabel.Position - Position;
        Size = _RTlabel.Size + new Vector2(1.5f, 2) * difference.Abs();
    }

    public void SetEntireVisibility(bool visible){
        _RTlabel.Visible = visible;
        Visible = visible;
    }
}
