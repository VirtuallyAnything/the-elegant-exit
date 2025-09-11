using Godot;
using System;

namespace tee
{
	[GlobalClass]
	public abstract partial class PlayerAttack : Attack
	{
		private CharacterName _owningCharacter;
		private int _conversationInterestDamage;
		private bool _isFromItem;
		[Export]
		private BonusEffect _bonusEffect;

		[Export]
		public CharacterName OwningCharacter
		{
			get { return _owningCharacter; }
			set { _owningCharacter = value; }
		}
		/// <summary> Is always a positive value.</summary>
		[Export]
		public int ConversationInterestDamage
		{
			get { return _conversationInterestDamage; }
			set { _conversationInterestDamage = Math.Abs(value); }
		}
		/// <summary>
		/// True, if this attack was acquired by picking up an item.
		/// </summary>
		[Export]
		public bool IsFromItem
		{
			get { return _isFromItem; }
			set { _isFromItem = value; }
		}
		public BonusEffect BonusEffect
		{
			get { return _bonusEffect; }
		}

		public abstract string GetQuote();
	}
}
