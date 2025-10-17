using Godot;

namespace tee
{
	public partial class PauseOverlay : Control
	{
		[Export] private Control _popup;
		[Export] private Control _glossary;
		private void OnPauseButtonPressed()
		{
			GetTree().Paused = true;
			Visible = true;
		}

		private void OnGameResumed()
		{
			GetTree().Paused = false;
			Visible = false;
		}

		private void OnMainMenuButtonPressed()
		{
			_popup.Visible = true;
		}

		private void OnGlossaryButtonPressed()
		{
			_glossary.Visible = true;
		}

		private void OnGlossaryClosed()
		{
			_glossary.Visible = false;
		}

		private void OnCancelButtonPressed()
		{
			_popup.Visible = false;
		}
	}
}

