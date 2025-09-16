using Godot;
using System;
namespace tee
{
	public partial class GameTimer : Timer
	{
		[Export] private Label _displayLabel;
		[Export] private float _timeMinutes = 30;
		private TimeSpan time;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			float timeSeconds = _timeMinutes * 60;
			time = TimeSpan.FromSeconds(timeSeconds);
			Start(timeSeconds);
			_displayLabel.Text = time.ToString(@"mm\:ss");
			Timeout += GameManager.EndGame;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			time = TimeSpan.FromSeconds(TimeLeft);
			_displayLabel.Text = time.ToString(@"mm\:ss");
		}
	}
}
