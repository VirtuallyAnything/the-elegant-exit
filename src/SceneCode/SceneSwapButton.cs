using Godot;
using System;
using tee;

public partial class SceneSwapButton : BaseButton
{
	private SceneManager _sceneManager;
    [Export] private SceneName _sceneName;

    public override void _Ready()
    {
		_sceneManager = GetNode("/root/SceneManager") as SceneManager;
        Pressed += OnPressed;
    }

    public void OnPressed(){
        _sceneManager.ChangeToScene(_sceneName);
    }
}
