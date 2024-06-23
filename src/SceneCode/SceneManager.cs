using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public partial class SceneManager : Node
	{
		
		private static EncounterScene _encounterScene = new();
		private EncounterStartScreen _encounterStartScene = new();
		private static PartyGroundFloor _partyGroundFloor;
		private static PartyFirstFloor _partyFirstFloor;
		private static MainScene _mainScene;
		public static EncounterScene EncounterScene
		{
			get { return _encounterScene; }
			set { _encounterScene = value; }
		}
		public static PartyGroundFloor PartyGroundFloor
		{
			get { return _partyGroundFloor; }
			set { _partyGroundFloor = value; }
		}
		public static MainScene MainScene
		{
			get { return _mainScene; }
			set { _mainScene = value; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CombatManager.CombatEnded += ExitEncounter;
			GameManager.GameOver += ChangeToGameOverScene;
			_encounterStartScene = (EncounterStartScreen) ResourceLoader.Load<PackedScene>("res://Scenes/EncounterStartScreen.tscn").Instantiate();
			_partyGroundFloor = (PartyGroundFloor) ResourceLoader.Load<PackedScene>("res://Scenes/PartyGroundFloor.tscn").Instantiate();
			_partyFirstFloor = (PartyFirstFloor) ResourceLoader.Load<PackedScene>("res://Scenes/PartyFirstFloor.tscn").Instantiate();
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
			MainScene.RemoveChild(MainScene.CurrentFloor);
			MainScene.AddChild(_partyGroundFloor);
			MainScene.MoveChild(_partyGroundFloor, 0);
			MainScene.CurrentFloor = _partyGroundFloor;
		}

		private void ChangeToPartyFirstFloor()
		{
			MainScene.RemoveChild(MainScene.CurrentFloor);
			MainScene.AddChild(_partyFirstFloor);
			MainScene.MoveChild(_partyFirstFloor, 0);
			MainScene.CurrentFloor = _partyFirstFloor;
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
			_mainScene.CurrentFloor.Visible = false;
			_mainScene.CurrentFloor.SetProcess(false);
			_mainScene.EncounterLayer.AddChild(_encounterStartScene);
		}

		public static void ChangeToEncounterScene()
		{
			_mainScene.CurrentFloor.Visible = false;
			_mainScene.CurrentFloor.SetProcess(false);
			_mainScene.EncounterLayer.AddChild(_encounterScene);
			_encounterScene.SetupScene(GameManager.CurrentEnemy);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
		}

		public static void ExitEncounter()
		{
			_mainScene.CurrentFloor.ProcessMode = ProcessModeEnum.Pausable;
			_mainScene.UpdateUI();
			_mainScene.CurrentFloor.Visible = true;
			_mainScene.EncounterLayer.RemoveChild(_encounterScene);
		}

		public override void _ExitTree()
		{
			CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
		}
	}
}
