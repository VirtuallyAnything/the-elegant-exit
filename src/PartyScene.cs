using Godot;
using System;

namespace tee
{
	public partial class PartyScene : Node2D
	{
		[Export] private TextureProgressBar _socialBattery;
		public override void _Ready()
		{
			SceneManager.PartyScene = this;
			_socialBattery.Value = GameManager.SocialBattery;
		}

		public void OnSceneReentered()
		{
			_socialBattery.Value = GameManager.SocialBattery;
		}
	}
}
