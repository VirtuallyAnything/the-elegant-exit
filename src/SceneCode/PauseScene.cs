using Godot;
using System;

public partial class PauseScene : Control
{
	[Export] private Control _popup;
		private void OnPauseButtonPressed()
		{
			GetTree().Paused = true;
			Visible = true;
		}

		private void OnGameResumed(){
			GetTree().Paused = false;
			Visible = false;
		}

		private void OnMainMenuButtonPressed(){
			_popup.Visible = true;
		}

		private void OnCancelButtonPressed(){
			_popup.Visible = false;
		}
}
