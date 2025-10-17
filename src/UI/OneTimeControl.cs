using Godot;
using System;

public partial class OneTimeControl : Control
{
	public override void _Ready()
	{
		GetTree().Paused = true;
	}

	private void Close()
	{
		Visible = false;
		GetTree().Paused = false;
		QueueFree();
	}
}
