using System.Linq;
using Godot;

namespace tee
{
	[GlobalClass]
	public partial class AttackCardContainer : Container
	{
		private AttackCard _frontMostAttackCard;
		private PackedScene _attackCardFront;
		private PackedScene _attackCardBack;

		public override void _Ready()
		{
			TopicButton.OnButtonPressed += DisableInput;
			CombatManager.EnemyTurnComplete += EnableInput;
			_attackCardFront = GD.Load<PackedScene>("res://Scenes/Subscenes/AttackCardFront.tscn");
			_attackCardBack = GD.Load<PackedScene>("res://Scenes/Subscenes/AttackCardBack.tscn");
			DisableInput();
		}

		public override void _Notification(int what)
		{
			if (what == NotificationSortChildren)
			{
				foreach (Control child in GetChildren())
				{
					int index = child.GetIndex();
					Vector2 position = new Vector2(0, child.Size.Y / 5 * index);

					Tween tween = GetTree().CreateTween();
					tween.TweenProperty(child, $"{Control.PropertyName.Position}", position, 0.5);
				}
			}
		}

		public void AddNewAttackCard(PlayerAttack attack)
		{
			AttackCard attackCard = new(attack, _attackCardFront.Instantiate<AttackCardFront>(), _attackCardBack.Instantiate<AttackCardBack>());
			AddChild(attackCard);
			attackCard.AttackCardPressed += OnAttackCardPressed;
			_frontMostAttackCard = attackCard;
		}

		public void SwapAttackCardOutFor(PlayerAttack newAttack)
		{
			_frontMostAttackCard.AttackCardPressed -= OnAttackCardPressed;
			RemoveChild(_frontMostAttackCard);
			_frontMostAttackCard.QueueFree();
			AddNewAttackCard(newAttack);
			Update();
		}

		public void Update()
        {
            foreach (AttackCard child in GetChildren())
			{
				child.Update();
			}
        }

		private void OnAttackCardPressed(AttackCard attackCard)
		{
			if (attackCard.Equals(_frontMostAttackCard))
			{
				if (!attackCard.IsOneSided)
				{
					attackCard.Flip();
				}
				else
				{
					attackCard.Select();
					DisableInput();
				}
			}
			else
			{
				if (_frontMostAttackCard.CurrentOpenSide is AttackCardBack)
				{
					_frontMostAttackCard.Flip();
				}
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(
					attackCard, $"{Control.PropertyName.Position}", new Vector2(0, -(attackCard.Size.Y / 5 * (attackCard.GetIndex() + 4))), 1);
				tween.TweenCallback(Callable.From(() => MoveChild(attackCard, GetChildCount() - 1)));

				_frontMostAttackCard = attackCard;
			}
		}

		public void DisableInput()
		{
			Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
			PropagateCall("set_mouse_filter", [(int)Control.MouseFilterEnum.Ignore]);
		}

		public void EnableInput()
		{
			Modulate = new Color(1, 1, 1, 1);
			PropagateCall("set_mouse_filter", [(int)Control.MouseFilterEnum.Pass]);
			foreach (Control child in GetChildren().Cast<Control>())
			{
				child.MouseFilter = MouseFilterEnum.Stop;
			}
		}

		public override void _ExitTree()
		{
			AttackCardBack.ClearPreferences();
			TopicButton.OnButtonPressed -= DisableInput;
			CombatManager.EnemyTurnComplete -= EnableInput;
		}
	}
}
