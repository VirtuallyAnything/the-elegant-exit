using Godot;
using System;

public partial class PartyFirstFloor : PartyFloor
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void OnFloorEntered()
    {
		//Enable an Area 2D that when triggered in turn enables the SceneSwitchArea on the Staircase
    }

}
