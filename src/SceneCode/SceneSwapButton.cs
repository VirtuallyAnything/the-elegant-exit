using Godot;
using System;
using tee;

public partial class SceneSwapButton : BaseButton
{
	private SceneManager _sceneManager;
    private SceneName _sceneName;
    [Export]
    public SceneName SceneName{
        get{return _sceneName;}
        set{_sceneName = value;}
    }

    public override void _Ready()
    {
		_sceneManager = GetNode("/root/SceneManager") as SceneManager;
        Pressed += OnPressed;
    }

    public void OnPressed(){
        _sceneManager.ChangeToScene(_sceneName);
    }
}
