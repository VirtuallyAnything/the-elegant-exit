using Godot;
using System;
using System.Runtime.CompilerServices;

namespace tee
{
	public partial class SceneManager : Node
	{
		private static EncounterScene _encounterScene;
		private static PartyScene _partyScene;
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

		public void ChangeToPartyScene()
		{
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/PartyScene.tscn");
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
			_partyScene.Visible = false;
			_partyScene.SetProcess(false);
			_canvasLayer.ProcessMode = ProcessModeEnum.Pausable;
			_canvasLayer.Visible = true;
			_canvasLayer.AddChild(scene.Instantiate());
			_encounterScene.SetupScene(enemyData);
			_encounterScene.LeaveButton.Pressed += ExitEncounter;
		}

		public static void ExitEncounter()
		{
			_partyScene.ProcessMode = ProcessModeEnum.Pausable;
			_partyScene.OnSceneReentered();
			_partyScene.Visible = true;
			_canvasLayer.SetProcess(false);
			_canvasLayer.Visible = false;
			_canvasLayer.GetChild(0).QueueFree();
		}

        public override void _ExitTree()
        {
            CombatManager.CombatEnded -= ExitEncounter;
			GameManager.GameOver -= ChangeToGameOverScene;
        }
    }
}
