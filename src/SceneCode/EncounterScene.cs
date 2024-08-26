using Godot;

namespace tee
{
	public delegate void EncounterSceneHandler();
	public partial class EncounterScene : Scene
	{
		public event EncounterSceneHandler SetupCompleted;
		public static event EncounterSceneHandler PlayerTurnComplete;
		public static event EncounterSceneHandler EnemyTurnComplete;

		[Export] private TextureRect _playerSprite;
		[Export] private Color _playerDialogueColor;
		[Export] private VariableDialogueButton _playerDialogue;

		[Export] private TextureRect _enemySprite;
		[Export] private Color _enemyDialogueColor;
		[Export] private VariableDialogueButton _enemyDialogue;
		[Export] private Label _enemyName;
		private EnemyData _currentEnemy;

		private Tween _activeDialogueTween;
		private bool _dialogueAnimationFinished = true;
		private bool _attackAnimationFinished = true;
		private bool _isPlayerTurn;
		[Export] private AttackCardContainer _attackCardContainer;
		[Export] private AnimationPlayer _animationPlayer;
		[Export] private Label _mentalCapacityValue;
		[Export] private Label _conversationInterestValue;
		[Export] private Label _socialStandingValue;
		[Export] private TextureProgressBar _socialBatteryProgress;
		[Export] private Button _leaveButton;

		public EnemyData CurrentEnemy
		{
			get { return _currentEnemy; }
			set { _currentEnemy = value; }
		}
		public Button LeaveButton
		{
			get { return _leaveButton; }
		}
		public AttackCardContainer AttackCardContainer
		{
			get { return _attackCardContainer; }
		}

		public void SetupScene(EnemyData enemyData)
		{
			_currentEnemy = enemyData;
			_enemyName.Text = _currentEnemy.DisplayName;
			_enemySprite.Texture = enemyData.Texture;
			_playerDialogue.Text = "";
			_enemyDialogue.Text = "";
			_playerDialogue.DialogueButtonPressed += OnSpeechBubblePressed;
			_enemyDialogue.DialogueButtonPressed += OnSpeechBubblePressed;
			//_dialogueLine.Modulate = _enemyDialogueColor;
			_socialBatteryProgress.Value = GameManager.SocialBattery;
			SetupCompleted?.Invoke();
		}

		public void PlayCombatAnimation(PlayerAttack attack)
		{
			_playerDialogue.SetEntireVisibility(true);
			_enemyDialogue.SetEntireVisibility(false);

			_playerDialogue.Text = attack.GetQuote();
			//_dialogueLine.Modulate = _playerDialogueColor;
			_dialogueAnimationFinished = false;
			_activeDialogueTween = _playerDialogue.CreateTween();
			int textLength = _playerDialogue.Text.Length;
			float animationLength = textLength * .05f;
			PropertyTweener propTweener = _activeDialogueTween.TweenProperty(
				_playerDialogue.RTLabel, $"{Label.PropertyName.VisibleCharacters}", textLength, animationLength);
			propTweener.From(0);
			_activeDialogueTween.TweenCallback(Callable.From(() => _dialogueAnimationFinished = true));
			_activeDialogueTween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
		}

		public void PlayCombatAnimation(EnemyAttack attack)
		{
			_enemyDialogue.SetEntireVisibility(true);
			_enemyDialogue.Disabled = false;
			_playerDialogue.SetEntireVisibility(false);
			_enemyDialogue.Disabled = false;
			_enemyDialogue.Text = attack.Quote;
			//_dialogueLine.Modulate = _enemyDialogueColor;
			Tween tween = _enemyDialogue.CreateTween();
			int textLength = _enemyDialogue.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_enemyDialogue.RTLabel, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
			propTweener.From(0);
			tween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
		}

		public void SkipDialogueAnimation(VariableDialogueButton variableDialogue)
		{
			if (_activeDialogueTween == null)
			{
				return;
			}
			_activeDialogueTween.Pause();
			_activeDialogueTween.CustomStep(10);
			variableDialogue.RTLabel.VisibleCharacters = -1;
			_activeDialogueTween.Kill();
		}

		public void OnSpeechBubblePressed(VariableDialogueButton variableDialogue)
		{
			if (!_dialogueAnimationFinished)
			{
				SkipDialogueAnimation(variableDialogue);
			}
			else if (!_attackAnimationFinished)
			{
				//SkipAttackAnimation()
			}
			else
			{
				if (_isPlayerTurn)
				{
					PlayerTurnComplete?.Invoke();
					_isPlayerTurn = false;
				}
			}
		}

		private void PlayAnimationsForAttack(PlayerAttack playerAttack)
		{
			_attackAnimationFinished = false;
			if (playerAttack.ConversationInterestDamage < 0)
			{
				// Play Negative Feedback Animation
			}
			else if (playerAttack.ConversationInterestDamage > 0)
			{
				//Play Positive Feedback Animation
			}

			/*if(playerAttack.SocialStandingChange < 0){
				// Play Negative Feedback Animation
			}else if(playerAttack.SocialStandingChange > 0){
				//Play Positive Feedback Animation
			}	*/
			//PlayerTurnAnimationComplete?.Invoke();

			//Let the player click again when all the animations have finished
			_attackAnimationFinished = true;
		}

		private void PlayAnimationsForAttack(EnemyAttack enemyAttack)
		{
			_attackAnimationFinished = false;
			if (enemyAttack.SocialBatteryChange < 0)
			{
				// Play Negative Feedback Animation	
			}
			else if (enemyAttack.SocialBatteryChange > 0)
			{
				//Play Positive Feedback Animation
			}

			if (enemyAttack.MentalCapacityChange < 0)
			{
				// Play Negative Feedback Animation
			}
			else if (enemyAttack.MentalCapacityChange > 0)
			{
				//Play Positive Feedback Animation
			}
			_attackAnimationFinished = true;
			EnemyTurnComplete?.Invoke();
			_enemyDialogue.Disabled = true;
			_isPlayerTurn = true;
		}

		public void UpdateUI(int socialBatteryNew, float socialStandingNew, float mentalCapacityNew, float interestNew)
		{
			Tween tween = _socialBatteryProgress.CreateTween();
			PropertyTweener propTweener = tween.TweenProperty(
				_socialBatteryProgress, $"{TextureProgressBar.PropertyName.Value}", socialBatteryNew, 1f);
			propTweener.From(_socialBatteryProgress.Value);

			_mentalCapacityValue.Text = $"{mentalCapacityNew}";
			_socialStandingValue.Text = $"{socialStandingNew}";
			_conversationInterestValue.Text = $"{interestNew}";
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			_playerDialogue.DialogueButtonPressed -= OnSpeechBubblePressed;
			_enemyDialogue.DialogueButtonPressed -= OnSpeechBubblePressed;
		}
	}
}
