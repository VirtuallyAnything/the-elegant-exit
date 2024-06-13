using Godot;
using System;

namespace tee
{
	public partial class Attack : Resource
	{
		private string _attackName;
		private string _dialogue;
		private string _animation;

		[Export]
		public string AttackName
		{
			get { return _attackName; }
			set { _attackName = value; }
		}
		[Export]
		public string Dialogue
		{
			get { return _dialogue; }
			set { _dialogue = value; }
		}
		[Export]
		public string Animation
		{
			get { return _animation; }
			set { _animation = value; }
		}
	}
}
