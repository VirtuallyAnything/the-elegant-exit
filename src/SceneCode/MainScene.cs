using Godot;

namespace tee
{
	public delegate void MainSceneHandler(PartyPlayer player);
	public partial class MainScene : Scene
	{
		public static event MainSceneHandler SetupCompleted;
		[Export] private Scene _currentFloor;
		[Export] private CanvasLayer _encounterLayer;
		[Export] private TextureProgressBar _socialBattery;
		[Export] private Camera2D _camera;
		[Export] private PartyPlayer _player;
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
			UpdateUI();
			_camera.MakeCurrent();
			SetupCompleted?.Invoke(_player);
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

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}