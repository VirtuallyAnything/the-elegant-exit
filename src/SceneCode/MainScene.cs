using Godot;
using System;

namespace tee
{
	public delegate void InitializationFinishedHandler();
	public partial class MainScene : Node2D
	{
		public static event InitializationFinishedHandler MainSceneInit;
		[Export] private Scene _currentFloor;
		[Export] private CanvasLayer _encounterLayer;
		[Export] private TextureProgressBar _socialBattery;
		[Export] private Camera2D _camera;
		[Export] private PartyPlayer _player;
		public PartyPlayer Player
		{
			get { return _player; }
			set { _player = value; }
		}
		public Scene CurrentFloor
		{
			get { return _currentFloor; }
			set { _currentFloor = value; }
		}
		public CanvasLayer EncounterLayer
		{
			get { return _encounterLayer; }
			set { _encounterLayer = value; }
		}

		public override void _Ready()
		{
			SceneManager.MainScene = this;
			GameManager.Player = _player;
			GameManager.SetupGame();
			UpdateUI();
			_camera.MakeCurrent();
			MainSceneInit?.Invoke();
		}

		public void ChangeSubScene(Scene scene)
		{
			if (_currentFloor is not null)
			{
				RemoveChild(_currentFloor);
			}
			AddChild(scene);
			MoveChild(scene, 0);
			_currentFloor = scene;
		}



		public void UpdateUI()
		{
			_socialBattery.Value = GameManager.SocialBattery;
		}
	}
}