using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public partial class SceneManager : Node
	{
		private static EncounterScene _encounterScene;
		private EncounterStartScreen _encounterStartScene;
		private EncounterFinishedScene _encounterFinishedScene;
		private static PartyGroundFloor _partyGroundFloor;
		private static PartyFirstFloor _partyFirstFloor;
		private static MainScene _mainScene;
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
			CombatManager.CombatEnded += ChangeToEncounterFinishedScene;
			GameManager.GameOver += ChangeToGameOverScene;	
		}

		public void ChangeToScene(SceneName sceneName)
		{
			//currentScene.Remove()
			switch (sceneName)
			{
				case SceneName.MainMenu:
					
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

		private void ChangeToMainScene()
		{
			_partyGroundFloor = (PartyGroundFloor) ResourceLoader.Load<PackedScene>("res://Scenes/PartyGroundFloor.tscn").Instantiate();
			_partyFirstFloor = (PartyFirstFloor) ResourceLoader.Load<PackedScene>("res://Scenes/PartyFirstFloor.tscn").Instantiate();
			MainScene.MainSceneInit += ChangeToPartyGroundFloor;
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/MainScene.tscn");
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

		private void ChangeToPauseScene()
		{

		}

		private void ChangeToEncounterStartScene()
		{
			_encounterStartScene = (EncounterStartScreen) ResourceLoader.Load<PackedScene>("res://Scenes/EncounterStartScreen.tscn").Instantiate();
			//_mainScene.CallDeferred(MethodName.RemoveChild, _mainScene.CurrentFloor);
			_mainScene.EncounterLayer.AddChild(_encounterStartScene);
			_mainScene.EncounterLayer.Visible = true;
			_encounterStartScene.SetProcess(true);
		}

		private void ChangeToEncounterScene()
		{
			_encounterScene = (EncounterScene) ResourceLoader.Load<PackedScene>("res://Scenes/EncounterScene.tscn").Instantiate();
			_mainScene.EncounterLayer.RemoveChild(_encounterStartScene);
			_encounterStartScene.QueueFree();
			_mainScene.EncounterLayer.AddChild(_encounterScene);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
			_encounterScene.SetupScene(GameManager.CurrentEnemy);
		}

		private void ChangeToEncounterFinishedScene(){
			_encounterFinishedScene = (EncounterFinishedScene) ResourceLoader.Load<PackedScene>("res://Scenes/EncounterFinishedScene.tscn").Instantiate();
			_mainScene.EncounterLayer.RemoveChild(_encounterScene);
			_encounterScene.QueueFree();
			_mainScene.EncounterLayer.AddChild(_encounterFinishedScene);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
		}

		public void ExitEncounter()
		{
			_mainScene.UpdateUI();
			_mainScene.EncounterLayer.RemoveChild(_encounterFinishedScene);
			_encounterFinishedScene.QueueFree();
			_mainScene.AddChild(_mainScene.CurrentFloor);
			_mainScene.MoveChild(_mainScene.CurrentFloor, 0);
		}

		private void ChangeToGameOverScene()
		{
			MainScene.MainSceneInit -= ChangeToPartyGroundFloor;
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/GameOverScene.tscn");
		}
		
		public override void _ExitTree()
		{
			CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
		}
	}
}
