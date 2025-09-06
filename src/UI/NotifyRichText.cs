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
}
