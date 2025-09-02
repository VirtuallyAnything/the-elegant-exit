using Godot;
using System;

namespace tee
{
	public partial class PartyPlayer : PartyCharacter
	{
		[Export] private PlayerData _data;
		[Export] private NavigationAgent2D _agent;
		[Export] private PointLight2D _discoveryLight;
		public PlayerData Data
		{
			get{return _data;}
		}

		public override void _Ready()
		{
			RemoveChild(_discoveryLight);
			_vision = new PlayerVision(_discoveryLight);

			_movement = new PlayerMovement(_agent, this, _vision)
			{
				Speed = _movementSpeed
			};
			base._Ready();
		}
	}
}
