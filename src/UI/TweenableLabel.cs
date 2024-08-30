using Godot;
using System;

public partial class TweenableLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void SetLabelText(int toNumber)
		{
			Text = $"{toNumber}";
		}
}
