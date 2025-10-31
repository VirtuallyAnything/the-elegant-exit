using Godot;
using System;

namespace tee
{
	public partial class SceneSwitchArea : Area2D
	{
		private SceneManager _sceneManager;
		[Export] private SceneName _sceneName;

		public override void _Ready()
		{
			BodyEntered += OnBodyEntered;
		}

		public void OnBodyEntered(Node2D body)
		{
			if (body.IsInGroup("Player"))
			{
				_sceneManager = GetNode("/root/SceneManager") as SceneManager;
				_sceneManager.ChangeToScene(_sceneName);
				//_sceneManager.CallDeferred("ChangeToScene", (int)_sceneName);
			}
		}
	}
}
