using Godot;
using System;
using Godot.Collections;

namespace tee
{
	public delegate void AttackHandler(PlayerAttack attack);
	public delegate void AttackCardHandler(AttackCard attackCard);
	public partial class AttackCard : Container
	{
		private bool _isOneSided = true;
		private AttackCardFront _front;
		private AttackCardBack _back;
		private AttackCardSide _currentOpenSide;
		private PlayerAttack _boundAttack;
		private TopicName _boundTopic;
		private Array<TopicButton> _topicButtonsItem = new();
		private Array<TopicName> _itemConversationTopics = new();
		public static event AttackHandler AttackSelected;
		// Emitted when an AttackCard is selected, that isn't the frontmost card
		public event AttackCardHandler AttackCardPressed;
		public bool IsOneSided
		{
			get { return _isOneSided; }
		}
		public AttackCardSide CurrentOpenSide
		{
			get { return _currentOpenSide; }
		}
		public PlayerAttack BoundAttack
		{
			get { return _boundAttack; }
			set { _boundAttack = value; }
		}
		public TopicName BoundTopic
		{
			get { return _boundTopic; }
			set { _boundTopic = value; }
		}

		public AttackCard(PlayerAttack attack, AttackCardFront attackCardFront, AttackCardBack attackCardBack)
		{
			if (attack is TopicalPlayerAttack)
			{
				_back = attackCardBack;
				_back.Setup(attack);
				_isOneSided = false;
			}
			_front = attackCardFront;
			_front.Setup(attack);
			Size = new Vector2(0, _front.TextureNormal.GetHeight());
			AddChild(_front);
			_currentOpenSide = _front;
			_boundAttack = attack;
		}

		public override void _Ready()
		{
			_front.Pressed += OnAttackCardPressed;
			if (_back != null)
			{
				_back.Pressed += OnAttackCardPressed;
				_back.TopicPicked += OnTopicButtonChildPressed;
			}
		}

		public void OnTopicButtonChildPressed(TopicName topicName)
		{
			if (BoundAttack is TopicalPlayerAttack topicalPlayerAttack)
			{
				topicalPlayerAttack.SelectedTopicName = topicName;
			}
			Select();
		}

		private void OnAttackCardPressed()
		{
			AttackCardPressed?.Invoke(this);
		}

		public void Flip()
		{
			Tween tween = GetTree().CreateTween();
			float animationLength = 0.5f;
			tween.TweenProperty(_currentOpenSide, $"{TextureButton.PropertyName.Scale}", new Vector2(0, _currentOpenSide.Scale.Y), animationLength);
			tween.TweenCallback(Callable.From(FlipBack));
		}

		private void FlipBack()
		{
			RemoveChild(_currentOpenSide);
			if (_currentOpenSide is AttackCardBack)
			{
				_currentOpenSide = _front;
			}
			else
			{
				_currentOpenSide = _back;
			}
			_currentOpenSide.Scale = new Vector2(0, _currentOpenSide.Scale.Y);
			AddChild(_currentOpenSide);
			Tween tween = GetTree().CreateTween();
			float animationLength = 0.5f;
			PropertyTweener propTweener = tween.TweenProperty(
				_currentOpenSide, $"{TextureButton.PropertyName.Scale}", Vector2.One, animationLength);
			propTweener.From(new Vector2(0, _currentOpenSide.Scale.Y));
		}

		public void Select()
		{
			AttackSelected?.Invoke(BoundAttack);
		}

		public override void _ExitTree()
		{
			if (_back != null)
			{
				_back.TopicPicked -= OnTopicButtonChildPressed;
			}
		}
	}
}
