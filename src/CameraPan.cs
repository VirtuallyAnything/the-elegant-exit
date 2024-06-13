using Godot;
using System;

public partial class CameraPan : Camera2D
{
	[Export] private Sprite2D gameMap;
	private Rect2 _mapLimits;
	private Rect2 _cameraBounds;

    public override void _Ready()
    {
        base._Ready();
		_mapLimits = gameMap.GetRect();
		_mapLimits.Position = gameMap.Position;
		LimitRight = (int)_mapLimits.End.X;
		LimitBottom = (int)_mapLimits.End.Y;
		
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		if(Input.IsActionPressed("PanCamera")){
			if(@event is InputEventMouseMotion screenDrag){
				screenDrag = (InputEventMouseMotion) @event;
				Position -= screenDrag.Relative;	
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
