using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public partial class SceneManager : Node
	{
		private static EncounterScene _encounterScene = new();
		private EncounterStartScreen _encounterStartScene = new();
		private static PartyScene _partyScene;
		private static MainScene _mainScene;
		private static CanvasLayer _canvasLayer;
		public static EncounterScene EncounterScene
		{
			get { return _encounterScene; }
			set { _encounterScene = value; }
		}
		public static PartyScene PartyScene
		{
			get { return _partyScene; }
			set { _partyScene = value; }
		}
		public static MainScene MainScene
		{
			get { return _mainScene; }
			set { _mainScene = value; }
		}
		public static CanvasLayer CanvasLayer
		{
			get { return _canvasLayer; }
			set { _canvasLayer = value; }
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CombatManager.CombatEnded += ExitEncounter;
			GameManager.GameOver += ChangeToGameOverScene;
		}

		public void ChangeToScene(SceneName sceneName)
		{
			//currentScene.Remove()
			switch (sceneName)
			{
				case SceneName.MainMenu:
					ChangeToMainScene();
					break;
				case SceneName.MainScene:
					ChangeToMainScene();
					break;
				case SceneName.PauseMenu:
					ChangeToPauseScene();
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
					break;
				case SceneName.GameOver:
					ChangeToGameOverScene();
					break;
			}
		}

		public void ChangeToMainScene()
		{
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/MainScene.tscn");
		}

		private void ChangeToPartyGroundFloor()
		{

		}

		private void ChangeToPartyFirstFloor()
		{

		}

		private void ChangeToPartySecondFloor()
		{

		}

		private void ChangeToPauseScene()
		{

		}

		private void ChangeToGameOverScene()
		{
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/GameOverScene.tscn");
		}

		private void ChangeToEncounterStartScene()
		{
			_mainScene.PartyLayer.Visible = false;
			_mainScene.PartyLayer.SetProcess(false);
			_mainScene.EncounterLayer.AddChild(_encounterStartScene);
		}

		public static void ChangeToEncounterScene()
		{
			_mainScene.PartyLayer.Visible = false;
			_mainScene.PartyLayer.SetProcess(false);
			_mainScene.EncounterLayer.AddChild(_encounterScene);
			_encounterScene.SetupScene(GameManager.CurrentEnemy);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
		}

		public static void ExitEncounter()
		{
			_mainScene.PartyLayer.ProcessMode = ProcessModeEnum.Pausable;
			_partyScene.OnSceneReentered();
			_mainScene.PartyLayer.Visible = true;
			_mainScene.EncounterLayer.RemoveChild(_encounterScene);
		}

		public override void _ExitTree()
		{
			CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
		}
	}
}
