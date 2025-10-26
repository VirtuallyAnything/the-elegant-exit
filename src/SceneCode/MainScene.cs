using Godot;

namespace tee
{
	public partial class MainScene : Scene
	{
		[Export] private int _currentFloorIndex;
		[Export] private Godot.Collections.Array<PackedScene> _floorsPacked;
		private Godot.Collections.Array<PartyFloorScene> _floors = new();
		[Export] private CanvasLayer _encounterLayer;
		[Export] private CanvasLayer _pauseLayer;
		private Node _currentEncounterScene;
		[Export] private TextureProgressBar _socialBattery;
		[Export] private Camera2D _camera;

		public CanvasLayer EncounterLayer
		{
			get { return _encounterLayer; }
			set { _encounterLayer = value; }
		}

		public CanvasLayer PauseLayer
		{
			get { return _pauseLayer; }
		}

		public override void _Ready()
		{
			UpdateUI();
			_camera.MakeCurrent();
			foreach(PackedScene packedScene in _floorsPacked)
            {
				_floors.Add(packedScene.Instantiate<PartyFloorScene>()); 
            }
			AddChild(_floors[_currentFloorIndex]);
			MoveChild(_floors[_currentFloorIndex], 0);
		}

		public void ChangeFloorUp()
		{
			if (_currentFloorIndex + 1 < _floors.Count)
			{
				RemoveChild(_floors[_currentFloorIndex]);
				AddChild(_floors[_currentFloorIndex++]);
				MoveChild(_floors[_currentFloorIndex], 0);
			}
		}

		public void ChangeFloorDown()
		{
			if (_currentFloorIndex - 1 >= 0)
			{
				RemoveChild(_floors[_currentFloorIndex]);
				AddChild(_floors[_currentFloorIndex--]);
				MoveChild(_floors[_currentFloorIndex], 0);
			}
		}

		public void ChangeEncounterScene(Node scene)
		{
			if (scene is not null)
			{
				_floors[_currentFloorIndex].ProcessMode = ProcessModeEnum.Disabled;
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
			_floors[_currentFloorIndex].ProcessMode = ProcessModeEnum.Inherit;
		}

		public void UpdateUI()
		{
			_socialBattery.Value = GameManager.SocialBattery;
		}
	}
}