using Godot;
using System;
using tee;

public partial class MainScene : Node2D
{
	[Export] private PartyFloor _currentFloor;
	[Export] private CanvasLayer _encounterLayer;
	[Export] private TextureProgressBar _socialBattery;
	[Export] private Camera2D _camera;
	public PartyFloor CurrentFloor
	{
		get { return _currentFloor; }
		set { _currentFloor = value; }
	}
	public CanvasLayer EncounterLayer
	{
		get { return _encounterLayer; }
		set { _encounterLayer = value; }
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SceneManager.MainScene = this;
		UpdateUI();
	}

	public void UpdateUI()
	{
		_socialBattery.Value = GameManager.SocialBattery;
		_camera.MakeCurrent();
	}
}
