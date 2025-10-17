using Godot;

namespace tee
{
	public partial class MainScene : Node
	{
		[Export] private Node _currentFloor;
		[Export] private CanvasLayer _encounterLayer;
		private Node _currentEncounterScene;
		[Export] private TextureProgressBar _socialBattery;
		[Export] private Camera2D _camera;
		public Node CurrentFloor
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
		}

		public void ChangeSubScene(Node scene)
		{
			if (scene is not null)
			{
				if (_currentFloor is not null)
				{
					RemoveChild(_currentFloor);
				}
				AddChild(scene);
				MoveChild(scene, 0);
				_currentFloor = scene;
			}
		}

		public void ChangeEncounterScene(Node scene)
		{
			if (scene is not null)
			{
				if (_currentEncounterScene is not null)
				{
					_encounterLayer.RemoveChild(_currentEncounterScene);
				}
				_encounterLayer.AddChild(scene);
				_currentEncounterScene = scene;
			}
		}

		public void RemoveEncounter()
		{
			if (_currentEncounterScene is not null)
			{
				_encounterLayer.RemoveChild(_currentEncounterScene);
			}
			_currentEncounterScene = null;
		}

		public void UpdateUI()
		{
			_socialBattery.Value = GameManager.SocialBattery;
		}
	}
}