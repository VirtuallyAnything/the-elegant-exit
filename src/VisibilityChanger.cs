using Godot;
using System;

public partial class VisibilityChanger : Container
{
	public void ShowContainer(bool visibility){
		
		Visible = visibility;
	}
}
