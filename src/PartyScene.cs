using Godot;
using System;

public partial class PartyScene : Node2D
{
	[Export] private TextureProgressBar _socialBattery;
	public override void _Ready()
	{
		SceneManager.CanvasLayer = GetChild<CanvasLayer>(0);
		SceneManager.PartyScene = this;
		_socialBattery.Value = GameManager.PlayerData.SocialBattery;
	}

	public void OnSceneReentered(){
		_socialBattery.Value = GameManager.PlayerData.SocialBattery;
	}
}
