using Godot;
using System;

public partial class ButtonGrid : GridContainer
{
	private Godot.Collections.Array<Button> _childButtons = new();
	public Godot.Collections.Array<Button> ChildButtons{
		get{return _childButtons;}
	}
	public override void _Ready()
	{
	}

	public void DisableAllButtons(){
		foreach(Button button in _childButtons){
			button.Disabled = true;
		}
	}

	public void Add(Button button){
		AddChild(button);
		_childButtons.Add(button);
	}
}
