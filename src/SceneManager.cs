using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public partial class SceneManager : Node
	{
		private static EncounterScene _encounterScene;
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

		public void ChangeToScene(SceneName sceneName){
			switch (sceneName)
			{
				case SceneName.MainMenu: ChangeToMainScene();
				break;
    			case SceneName.PauseMenu: ChangeToPauseScene();
				break;
    			case SceneName.Scoreboard:
				break;
    			case SceneName.NicknameScreen:
				break;
    			case SceneName.PartyGroundFloor:
				break;
    			case SceneName.PartyFirstFloor:
				break;
    			case SceneName.PartySecondFloor:
				break;
    			case SceneName.EncounterStart:
				break;
    			case SceneName.Encounter:
				break;
    			case SceneName.EncounterFinished:
				break;
    			case SceneName.GameOver:
				break;
			}
		}

		public void ChangeToMainScene()
		{
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/MainScene.tscn");
		}

		public void ChangeToPauseScene()
		{

		}

		private void ChangeToGameOverScene(){
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/GameOverScene.tscn");
		}

		public static void ChangeToEncounterScene(EnemyData enemyData)
		{
			PackedScene scene = GD.Load<PackedScene>("res://Scenes/EncounterScene.tscn");
			_mainScene.PartyLayer.Visible = false;
			_mainScene.PartyLayer.SetProcess(false);
			_mainScene.EncounterLayer.ProcessMode = ProcessModeEnum.Pausable;
			_mainScene.EncounterLayer.Visible = true;
			_mainScene.EncounterLayer.AddChild(scene.Instantiate());
			_encounterScene.SetupScene(enemyData);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
		}

		public static void ExitEncounter()
		{
			_mainScene.PartyLayer.ProcessMode = ProcessModeEnum.Pausable;
			_partyScene.OnSceneReentered();
			_mainScene.PartyLayer.Visible = true;
			_mainScene.EncounterLayer.SetProcess(false);
			_mainScene.EncounterLayer.Visible = false;
			_mainScene.EncounterLayer.GetChild(0).QueueFree();
		}

        public override void _ExitTree()
        {
            CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
        }
    }
}
