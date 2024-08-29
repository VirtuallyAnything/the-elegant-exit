using Godot;
using Godot.Collections;

namespace tee
{
	public partial class StartScene : Scene
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			foreach (Node child in GetChildren())
			{
				if (child is SceneSwapButton button)
				{
					switch (child.Name)
					{
						case "PlayButton":
							button.SceneName = SceneName.MainScene;
							break;
						case "Scoreboard":
							button.SceneName = SceneName.Scoreboard;
							break;
						case "Credits":
							button.SceneName = SceneName.Credits;
							break;
					}
				}
			}
		}

	}
}
