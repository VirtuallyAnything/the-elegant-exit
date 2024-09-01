using Godot;
using System;

public partial class OneTimePanel : Panel
{

	private void OnStartButtonPressed(){
		Visible = false;
		GetTree().Paused = false;
		QueueFree();
	}
}
