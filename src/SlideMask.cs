using Godot;
using System;

public partial class SlideMask : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Control childControl = GetChild<Control>(0);
		Size = childControl.Size;
	}

}
