using Godot;

namespace tee
{
	[GlobalClass, Tool]
	public partial class AttackCardContainer : Container
	{
		private AttackCard _frontMostAttackCard;

		public override void _Ready()
		{
			TopicButton.OnButtonPressed += DisableInput;
			EncounterScene.EnemyTurnAnimationComplete += EnableInput;
		}

		public override void _Notification(int what)
		{
			if (what == NotificationSortChildren)
			{
				// Must re-sort the children
				foreach (Control child in GetChildren())
				{
					int index = child.GetIndex();
					child.Position = Vector2.Zero;
					child.Position = new Vector2(0, child.Size.Y / 5 * index);
					GD.Print($"Position: {child.Position}, Index: {index}");
				}
			}
		}

		public void AddNewAttackCard(PlayerAttack attack)
		{
			AttackCard attackCard = new(attack);
			AddChild(attackCard);
			_frontMostAttackCard = attackCard;
		}

		public void SwapAttackCardOutFor(PlayerAttack newAttack)
		{
			RemoveChild(_frontMostAttackCard);
			AddNewAttackCard(newAttack);
		}

		public void DisableInput()
		{
			PropagateCall("set_mouse_filter", [(int)Control.MouseFilterEnum.Ignore]);
		}

		public void EnableInput()
		{
			PropagateCall("set_mouse_filter", [(int)Control.MouseFilterEnum.Stop]);
		}

		public override void _ExitTree()
		{
			TopicButton.OnButtonPressed -= DisableInput;
			EncounterScene.EnemyTurnAnimationComplete -= EnableInput;
		}
	}
}
