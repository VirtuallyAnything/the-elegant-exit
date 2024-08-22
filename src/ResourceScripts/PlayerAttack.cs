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
		private BonusEffect _bonusEffect;
		
		[Export]
        public CharacterName OwningCharacter
        {
			get{return _owningCharacter;}
			set{_owningCharacter = value;}
		}
		[Export]
		/// <summary> Is always a positive value.</summary>
		public int ConversationInterestDamage
		{
			get { return _conversationInterestDamage; }
			set { _conversationInterestDamage = Math.Abs(value); }
		}
		[Export]
		public bool IsFromItem
		{
			get { return _isFromItem; }
			set { _isFromItem = value; }
		}
		public BonusEffect BonusEffect{
			get{return _bonusEffect;}
		}
	
		public abstract string GetQuote();
	}
}
