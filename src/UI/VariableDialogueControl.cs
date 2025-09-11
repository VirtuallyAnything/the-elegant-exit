using Godot;

public partial class VariableDialogueControl : Control
{
    [Export] private NotifyRichText _RTlabel;
    private Vector2 _initialSize;
    private Vector2 _padding;
    public NotifyRichText RTLabel
    {
        get { return _RTlabel; }
    }
    public string Text
    {
        get { return _RTlabel.Text; }
        set { _RTlabel.Text = value; }
    }

    public override void _Ready()
    {
        if (_RTlabel == null)
        {
            _RTlabel = new();
            AddChild(_RTlabel);
        }
        _RTlabel.SizePropertyChanged += SetToLabelSize;
        _initialSize = Size;
        _padding = (_RTlabel.Position - Position).Abs();
    }

    private void SetToLabelSize()
    {
        Vector2 newSize = Size;
        newSize.Y = _RTlabel.Size.Y + 2 * _padding.Y;
        Size = newSize; 
    }

    public void SetEntireVisibility(bool visible){
        _RTlabel.Visible = visible;
        Visible = visible;
    }
}
