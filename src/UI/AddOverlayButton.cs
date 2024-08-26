using Godot;
using System;

public partial class AddOverlayButton : TextureButton
{
	[Export]
	private PackedScene _overlay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnButtonPressed;
	}

	public void OnButtonPressed(){
		AddSibling(_overlay.Instantiate());
	}
}
