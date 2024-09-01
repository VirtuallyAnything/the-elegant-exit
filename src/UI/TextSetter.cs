using Godot;
using System;

public partial class TextSetter : Label
{
	public void ValueChanged(float value){
		Text = $"{value}%";
	}
}
