using Godot;
using System;

namespace tee
{
	public partial class PartyGroundFloor : PartyFloor
	{
        public override void _Ready()
        {
            base._Ready();
        }
        public override void OnFloorEntered()
        {
            throw new NotImplementedException();
        }
	}
}
