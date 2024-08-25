using Godot;
using System;

public delegate void RichTextLabelHandler();
public partial class NotifyRichText : RichTextLabel
{
	public event RichTextLabelHandler SizePropertyChanged;

	public override void _Notification(int what)
	{
		base._Notification(what);
		if (what == NotificationResized)
		{
			SizePropertyChanged?.Invoke();
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
