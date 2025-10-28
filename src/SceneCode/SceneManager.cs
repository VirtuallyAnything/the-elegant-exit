using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace tee
{
	/// <summary>
	/// Master class in charge of all scene changes.
	/// </summary>
	public partial class SceneManager : Node
	{
		private static Scene _currentScene;
		private static MainScene _mainScene;
		private static bool _isSceneChanging;
		public static bool IsSceneChanging
		{
			get { return _isSceneChanging; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CombatManager.CombatEnded += ChangeToEncounterFinishedScene;
			GameManager.GameOver += ChangeToScene;
			TempScene.SceneTimerFinished += ChangeToScene;
			_currentScene = GetTree().Root.GetChild<Scene>(2);
		}

		public async Task ChangeToScene(SceneName sceneName)
		{
			_isSceneChanging = true;
			await _currentScene.TransitionOut();
			switch (sceneName)
			{
				case SceneName.Menu:
					RemoveAndInstantiate("res://Scenes/MenuScene.tscn");
					break;
				case SceneName.Credits:
					RemoveAndInstantiate("res://Scenes/CreditsScene.tscn");
					break;
				case SceneName.MainScene:
					RemoveAndInstantiate("res://Scenes/MainScene.tscn");
					_mainScene = (MainScene)_currentScene;
					break;
				case SceneName.Scoreboard:
					break;
				case SceneName.PartyFloorUp:
					_mainScene.ChangeFloorUp();
					break;
				case SceneName.PartyFloorDown:
					_mainScene.ChangeFloorDown();
					break;
				case SceneName.EncounterStart:
					await ChangeToEncounterStartScene();
					break;
				case SceneName.Encounter:
					await ChangeToEncounterScene();
					break;
				// For debugging purposes
				case SceneName.EncounterFinished:
					ChangeToEncounterFinishedScene(EncounterOutcome.EnemyDefeated, 0, 0);
					break;
				case SceneName.CurrentPartyFloor:
					ExitEncounter();
					break;
				case SceneName.GameOver:
					RemoveAndInstantiate("res://Scenes/GameOverScene.tscn");
					break;
				case SceneName.GameFinished:
					RemoveAndInstantiate("res://Scenes/GameFinishedScene.tscn");
					((GameFinishedScene)_currentScene).DisplayScore(GameManager.GetScore());
					break;
			}
			await _currentScene.TransitionIn();
			_isSceneChanging = false;
		}

		private void RemoveAndInstantiate(string sceneToInstantiate)
		{
			GetTree().Root.CallDeferred("remove_child", _currentScene);
			_currentScene = ResourceLoader.Load<PackedScene>(sceneToInstantiate).Instantiate<Scene>();
			GetTree().Root.CallDeferred("add_child", _currentScene);
		}

		private async Task ChangeToEncounterStartScene()
		{
            EncounterStartScene encounterScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterStartScene.tscn").Instantiate<EncounterStartScene>();
            await _mainScene.ChangeEncounterScene(encounterScene);
		}

		private async Task ChangeToEncounterScene()
		{
			EncounterScene encounterScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterScene.tscn").Instantiate<EncounterScene>();
			await _mainScene.ChangeEncounterScene(encounterScene);
			encounterScene.SetupScene(GameManager.CurrentEnemy);
			if (GameManager.IsFirstEncounter)
			{
				Node tutorial = ResourceLoader.Load<PackedScene>("res://Scenes/Subscenes/TutorialConfirmationWindow.tscn").Instantiate();
				_mainScene.PauseLayer.AddChild(tutorial);
			}	
		}

		private void ChangeToEncounterFinishedScene(EncounterOutcome outcome, int socialStanding, int socialBattery)
		{
			EncounterFinishedScene encounterFinishedScene = ResourceLoader.Load<PackedScene>("res://Scenes/EncounterFinishedScene.tscn").Instantiate<EncounterFinishedScene>();
			encounterFinishedScene.DisplayOutcome(outcome, socialStanding, socialBattery);
			_mainScene.ChangeEncounterScene(encounterFinishedScene);
		}

		public void ExitEncounter()
		{
			_mainScene.UpdateUI();
			_mainScene.RemoveEncounter();
		}

		public override void _ExitTree()
		{
			CombatManager.CombatEnded -= ChangeToEncounterFinishedScene;
			GameManager.GameOver -= ChangeToScene;
			TempScene.SceneTimerFinished -= ChangeToScene;
			_mainScene = null;
			_currentScene = null;
		}
	}
}
