using Godot;
using System;
using tee;

public partial class MainScene : Node2D
{
	[Export] private CanvasLayer _partyLayer;
	[Export] private CanvasLayer _encounterLayer;
	public CanvasLayer PartyLayer{
		get{return _partyLayer;}
		set{_partyLayer = value;}
	}
	public CanvasLayer EncounterLayer 
	{ 
		get{return _encounterLayer;}
		set{_encounterLayer = value;} 
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SceneManager.MainScene = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
