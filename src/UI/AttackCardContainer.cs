using Godot;
using Godot.Collections;
using tee;

public partial class AttackCardContainer : Container
{
	private AttackCard _frontMostAttackCard;


	// Called when the node enters the scene tree for the first time.
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
			foreach (Control c in GetChildren())
			{
				int index = c.GetIndex();
				c.Position = new Vector2(0, c.Size.Y / 5 * index);
			}
		}
	}

	public void AddNewAttackCard(PlayerAttack attack){
		AttackCard attackCard = new(attack);
		AddChild(attackCard);
		_frontMostAttackCard = attackCard;
	}

	public void SwapAttackCardOutFor(PlayerAttack newAttack){
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
