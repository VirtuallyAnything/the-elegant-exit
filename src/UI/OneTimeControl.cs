using Godot;
using System;

public partial class OneTimeControl : Control
{
	private void Close(){
		Visible = false;
		GetTree().Paused = false;
		QueueFree();
	}
}
