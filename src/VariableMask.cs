using Godot;
using System;

public partial class VariableMask : Control
{
	[Export] private Node2D _nodeToMask;
	[Export] private Texture2D _maskTexture;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void CreateMaskForString(string text){
		string[] textLines = text.Split("\n");
		TextureRect[] masks = new TextureRect[textLines.Length];
		VBoxContainer maskContainer = new VBoxContainer();
		
		for(int i = 0; i < textLines.Length; i++){
			RichTextLabel label = new RichTextLabel();
			label.Text = textLines[i];
			TextureRect mask = new TextureRect();
			mask.Texture = _maskTexture;
			mask.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
			mask.ClipChildren = ClipChildrenMode.AndDraw;
			mask.Size = label.Size;
			mask.AddChild(label);
			masks[i] = mask;
			maskContainer.AddChild(mask);
		}
			
		AddChild(maskContainer);
	}
}
