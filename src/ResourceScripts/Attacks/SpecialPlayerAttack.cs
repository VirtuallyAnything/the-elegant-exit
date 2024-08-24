using Godot;
using System;

namespace tee
{
	[GlobalClass]
	public partial class SpecialPlayerAttack : PlayerAttack
	{
		[Export]
		private Godot.Collections.Array<string> _quotes = new();

		public override string GetQuote()
		{
			return _quotes.PickRandom();
		}
	}
}
