using Godot;
using System;
using tee;

public partial class SceneSwapButton : Button
{
	private SceneManager sceneManager;

    public override void _Ready()
    {
		sceneManager = GetNode("/root/SceneManager") as SceneManager;
        Pressed += sceneManager.ChangeToMainScene;
    }
}
