using System.Collections.Generic;
using Godot;

namespace tee
{
	/// <summary>
	/// Master class in charge of all scene changes.
	/// </summary>
	public partial class SceneManager : Node
	{
		private static Node _currentScene;
		private static PartyFloorScene _partyGroundFloor;
		private static PartyFloorScene _partyFirstFloor;
		private static MainScene _mainScene;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CombatManager.CombatWon += ChangeToEncounterFinishedScene;
			GameManager.GameOver += ChangeToGameOverScene;
			_currentScene = GetTree().Root.GetChild<Node>(2);
		}

		public void ChangeToScene(SceneName sceneName)
		{
			switch (sceneName)
			{
				case SceneName.Menu:
					RemoveAndInstantiate("res://Scenes/MenuScene.tscn");
					break;
				case SceneName.Credits:
					RemoveAndInstantiate("res://Scenes/CreditsScene.tscn");
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
				// For debugging purposes
				case SceneName.EncounterFinished:
					ChangeToEncounterFinishedScene(true);
					break;
				case SceneName.CurrentPartyFloor:
					ExitEncounter();
					break;
				case SceneName.GameOver:
					ChangeToGameOverScene();
					break;
				case SceneName.GameFinished:
					ChangeToGameFinishedScene();
					break;
			}
		}

		private void RemoveAndInstantiate(string sceneToInstantiate)
		{
			GetTree().Root.CallDeferred("remove_child", _currentScene);

			_currentScene = ResourceLoader.Load<PackedScene>(sceneToInstantiate).Instantiate();
			GetTree().Root.CallDeferred("add_child", _currentScene);

		}

		private void ChangeToMainScene()
		{
			GetTree().Root.CallDeferred("remove_child", _currentScene);
			_mainScene = ResourceLoader.Load<PackedScene>("res://Scenes/MainScene.tscn").Instantiate<MainScene>();
			_partyGroundFloor = ResourceLoader.Load<PackedScene>("res://Scenes/PartyGroundFloor.tscn").Instantiate<PartyFloorScene>();
			_partyFirstFloor = ResourceLoader.Load<PackedScene>("res://Scenes/PartyFirstFloor.tscn").Instantiate<PartyFloorScene>();
			_mainScene.Ready += ChangeToPartyGroundFloor;
			_currentScene = _mainScene;
			GetTree().Root.CallDeferred("add_child", _currentScene);
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
			EncounterStartScene encounterScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterStartScene.tscn").Instantiate<EncounterStartScene>();
			_mainScene.ChangeEncounterScene(encounterScene);
			//_mainScene.EncounterLayer.Visible = true;
			//_encounterStartScene.SetProcess(true);
		}

		private void ChangeToEncounterScene()
		{
			EncounterScene encounterScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterScene.tscn").Instantiate<EncounterScene>();
			_mainScene.ChangeEncounterScene(encounterScene);
			encounterScene.SetupScene(GameManager.CurrentEnemy);
			if (GameManager.IsFirstEncounter)
			{
				Node tutorial = ResourceLoader.Load<PackedScene>("res://Scenes/Subscenes/TutorialConfirmationWindow.tscn").Instantiate();
				encounterScene.AddChild(tutorial);
			}
		}

		private void ChangeToEncounterFinishedScene(bool hasPlayerWon)
		{
			EncounterFinishedScene encounterFinishedScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterFinishedScene.tscn").Instantiate<EncounterFinishedScene>();
			_mainScene.ChangeEncounterScene(encounterFinishedScene);
			encounterFinishedScene.DisplayOutcome(hasPlayerWon);
		}

		public void ExitEncounter()
		{
			_mainScene.UpdateUI();
			_mainScene.RemoveEncounter();
		}

		private void ChangeToGameOverScene()
		{
			GetTree().Root.RemoveChild(_currentScene);
			_currentScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameOverScene.tscn").Instantiate();
			GetTree().Root.AddChild(_currentScene);
		}

		private void ChangeToGameFinishedScene()
		{
			GetTree().Root.RemoveChild(_currentScene);
			_currentScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameFinishedScene.tscn").Instantiate();
			((GameFinishedScene)_currentScene).DisplayScore(GameManager.GetScore());
			GetTree().Root.AddChild(_currentScene);
		}

		public override void _ExitTree()
		{
			CombatManager.CombatWon -= ChangeToEncounterFinishedScene;
			GameManager.GameOver -= ChangeToGameOverScene;
			QueueFree();
		}
	}
}
