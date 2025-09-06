using Godot;

namespace tee
{
	/// <summary>
	/// Master class in charge of all scene changes.
	/// </summary>
	public partial class SceneManager : Node
	{
		private static Scene _currentScene;
		private static EncounterScene _encounterScene;
		private EncounterStartScene _encounterStartScene;
		private EncounterFinishedScene _encounterFinishedScene;
		private static PartyFloorScene _partyGroundFloor;
		private static PartyFloorScene _partyFirstFloor;
		private static MainScene _mainScene;
		private static StartScene _mainMenu;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CombatManager.CombatEnded += ChangeToEncounterFinishedScene;
			GameManager.GameOver += ChangeToGameOverScene;
			_mainMenu = GetTree().Root.GetChild<StartScene>(3);
			_currentScene = _mainMenu;
		}

		public void ChangeToScene(SceneName sceneName)
		{
			switch (sceneName)
			{
				case SceneName.MainMenu:
					ChangeToMainMenu();
					break;
				case SceneName.Credits:
					ChangeToCreditsScene();
					break;
				case SceneName.MainScene:
					ChangeToMainScene();
					break;
				case SceneName.Scoreboard:
					break;
				case SceneName.NicknameScreen:
					break;
				case SceneName.PartyGroundFloor:
					ChangeToPartyGroundFloor();
					break;
				case SceneName.PartyFirstFloor:
					ChangeToPartyFirstFloor();
					break;
				case SceneName.PartySecondFloor:
					ChangeToPartySecondFloor();
					break;
				case SceneName.EncounterStart:
					ChangeToEncounterStartScene();
					break;
				case SceneName.Encounter:
					ChangeToEncounterScene();
					break;
				case SceneName.EncounterFinished:
					ChangeToEncounterFinishedScene();
					break;
				case SceneName.CurrentPartyFloor:
					ExitEncounter();
					break;
				case SceneName.GameOver:
					ChangeToGameOverScene();
					break;
			}
		}

		private void ChangeToMainMenu()
		{
			GetTree().Paused = false;
			GetTree().Root.CallDeferred("remove_child", _currentScene);

			if (_partyGroundFloor is not null)
			{
				_partyGroundFloor.QueueFree();
				_partyFirstFloor.QueueFree();
			}
			_currentScene.QueueFree();
			_currentScene = _mainMenu;
			GetTree().Root.CallDeferred("add_child", _currentScene);
		}

		private void ChangeToCreditsScene()
		{
			GetTree().Root.RemoveChild(_currentScene);
			_currentScene = (Scene)ResourceLoader.Load<PackedScene>("res://Scenes/CreditsScene.tscn").Instantiate();
			GetTree().Root.AddChild(_currentScene);
		}

		private void ChangeToMainScene()
		{
			GetTree().Root.RemoveChild(_currentScene);
			_mainScene = (MainScene)ResourceLoader.Load<PackedScene>("res://Scenes/MainScene.tscn").Instantiate();
			_partyGroundFloor = (PartyFloorScene)ResourceLoader.Load<PackedScene>("res://Scenes/PartyGroundFloor.tscn").Instantiate();
			_partyFirstFloor = (PartyFloorScene)ResourceLoader.Load<PackedScene>("res://Scenes/PartyFirstFloor.tscn").Instantiate();
			_mainScene.Ready += ChangeToPartyGroundFloor;
			GetTree().Root.AddChild(_mainScene);
			_currentScene = _mainScene;
			GetTree().Paused = true;
		}

		private void ChangeToPartyGroundFloor()
		{
			_mainScene.ChangeSubScene(_partyGroundFloor);
		}

		private void ChangeToPartyFirstFloor()
		{
			_mainScene.ChangeSubScene(_partyFirstFloor);
		}

		private void ChangeToPartySecondFloor()
		{

		}

		private void ChangeToEncounterStartScene()
		{
			_encounterStartScene = (EncounterStartScene)ResourceLoader.Load<PackedScene>("res://Scenes/EncounterStartScene.tscn").Instantiate();
			_mainScene.EncounterLayer.AddChild(_encounterStartScene);
			_mainScene.EncounterLayer.Visible = true;
			_encounterStartScene.SetProcess(true);
		}

		private void ChangeToEncounterScene()
		{
			_encounterScene = (EncounterScene)ResourceLoader.Load<PackedScene>("res://Scenes/EncounterScene.tscn").Instantiate();
			_mainScene.EncounterLayer.RemoveChild(_encounterStartScene);
			_encounterStartScene.QueueFree();
			_mainScene.EncounterLayer.AddChild(_encounterScene);
			_encounterScene.SetupScene(GameManager.CurrentEnemy);
		}

		private void ChangeToEncounterFinishedScene()
		{
			_encounterFinishedScene = (EncounterFinishedScene)ResourceLoader.Load<PackedScene>("res://Scenes/EncounterFinishedScene.tscn").Instantiate();
			_mainScene.EncounterLayer.RemoveChild(_encounterScene);
			_encounterScene.QueueFree();
			_mainScene.EncounterLayer.AddChild(_encounterFinishedScene);
		}

		public void ExitEncounter()
		{
			_mainScene.UpdateUI();
			_mainScene.EncounterLayer.RemoveChild(_encounterFinishedScene);
			_encounterFinishedScene.QueueFree();
		}

		private void ChangeToGameOverScene()
		{
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/GameOverScene.tscn");
		}

		public override void _ExitTree()
		{
			CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
			_currentScene.QueueFree();
			QueueFree();
		}
	}
}
