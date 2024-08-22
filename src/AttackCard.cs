using Godot;
using System;
using Godot.Collections;

namespace tee
{
	public delegate void AttackCardHandler(AttackCard attackCard);
	public partial class AttackCard : Control
	{
		private bool _isOnTop;
		private AttackCardFront _front;
		private AttackCardBack _back;
		private AttackCardSide _currentOpenSide;
		private PlayerAttack _boundAttack;
		private TopicName _boundTopic;
		[Export] AttackValueSetter _attackValueSetter;
		private Array<TopicButton> _topicButtonsItem = new();
		private Array<TopicName> _itemConversationTopics = new();
		public static event AttackCardHandler AttackSelected;
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

		public AttackCard(PlayerAttack attack)
		{
			if (attack is TopicalPlayerAttack topicalPlayerAttack)
			{
				_back = new(topicalPlayerAttack);
			}
			_front = new(attack);
			_boundAttack = attack;
		}

		public override void _Ready()
		{
			_front.Pressed += OnButtonPressed;
			if (_back != null)
			{
				_back.Pressed += OnButtonPressed;
				_back.TopicPicked += SetTopic;
			}
		}

		public void ChildPressed(TopicButton button)
		{
			_boundTopic = button.ConversationTopic;
			AttackSelected?.Invoke(this);
		}

		private void OnButtonPressed()
		{
			if (_isOnTop)
			{
				Flip();
			}
		}

		private void Flip()
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
			AddChild(_currentOpenSide);
		}

		private void SetTopic(TopicName topicName)
		{
			if (BoundAttack is TopicalPlayerAttack topicalPlayerAttack)
			{
				topicalPlayerAttack.SelectedTopicName = topicName;
			}
		}

		public override void _ExitTree()
		{
			_back.TopicPicked -= SetTopic;
		}


	}
}
