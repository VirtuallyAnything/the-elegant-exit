using System.Threading.Tasks;
using Godot;

namespace tee
{
	public delegate void EncounterSceneHandler();
	public partial class EncounterScene : Scene
	{
		public event EncounterSceneHandler SetupCompleted;
		public static event EncounterSceneHandler PlayerTurnComplete;

		[Export] private TextureRect _playerSprite;
		[Export] private Color _playerDialogueColor;
		[Export] private VariableDialogueControl _playerDialogue;

		[Export] private AnnoyanceDisplay _annoyanceDisplay;
		[Export] private TextureRect _enemySprite;
		[Export] private Color _enemyDialogueColor;
		[Export] private VariableDialogueControl _enemyDialogue;
		[Export] private Label _enemyName;
		private EnemyData _currentEnemy;

		private Label _labelToTweak;
		private Tween _activeDialogueTween;
		[Export] private AttackCardContainer _attackCardContainer;
		[Export] private AnimationPlayer _animationPlayer;

		[Export] private Label _currentTopic;
		[Export] private Label _annoyanceValue;
		[Export] private RichTextLabel _currentMaxCIChanges;

		[Export] private Label _mentalCapacityMax;
		[Export] private TweenableLabel _mentalCapacityValue;
		[Export] private Label _mentalCapacityDamage;

		[Export] private TweenableLabel _conversationInterestMax;
		[Export] private Label _conversationInterestMaxChange;
		[Export] private TweenableLabel _conversationInterestValue;
		private int _conversationInterestDelta;
		[Export] private Label _conversationInterestDamage;
		[Export] private TextureProgressBar _socialBatteryProgress;
		[Export] private Control _tutorialDialogue;
		public EnemyData CurrentEnemy
		{
			get { return _currentEnemy; }
			set { _currentEnemy = value; }
		}
		public AttackCardContainer AttackCardContainer
		{
			get { return _attackCardContainer; }
		}

		public void SetupScene(EnemyData enemyData)
		{
			ConversationTopic.EnthusiasmChangedForTopic += UpdateConversationInterest;
			AnnoyanceLevel.Changed += OnAnnoyanceChanged;
			EncounterCharacter.TopicChanged += UpdateTopic;
			_currentEnemy = enemyData;
			_conversationInterestMax.Text = $"{_currentEnemy.ConversationInterest}";
			_conversationInterestValue.Text = _conversationInterestMax.Text;
			_enemyName.Text = _currentEnemy.DisplayName;
			_enemySprite.Texture = enemyData.Texture;
			_playerDialogue.Text = "";
			_enemyDialogue.Text = "";
			_socialBatteryProgress.Value = GameManager.SocialBattery;
			SetupCompleted?.Invoke();
		}

		public async Task PlayCombatStartAnimation(int socialBatteryDifference)
		{
			_mentalCapacityMax.Text = $"{socialBatteryDifference}";
			_animationPlayer.Play("SocialBatterySubtract");
			await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

			Tween valueTween = _socialBatteryProgress.CreateTween();
			valueTween.Parallel().TweenProperty
			(
				_socialBatteryProgress,
				$"{TextureProgressBar.PropertyName.Value}",
				_socialBatteryProgress.Value - socialBatteryDifference,
				1.0f
			);
			valueTween.Parallel().TweenMethod
			(
				Callable.From<int>(_mentalCapacityValue.SetLabelText),
				0,
				socialBatteryDifference,
				1.0f
			);
			_animationPlayer.SpeedScale = -1;
			valueTween.TweenCallback(
				Callable.From(() => _animationPlayer.Play("SocialBatterySubtract", -1, 1, true)));
			await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}

		public async Task PlayDialogAnimation(PlayerAttack attack)
		{
			_playerDialogue.MouseFilter = Control.MouseFilterEnum.Ignore;
			_playerDialogue.SetEntireVisibility(true);
			_enemyDialogue.SetEntireVisibility(false);
			_playerDialogue.RTLabel.VisibleCharacters = 0;
			_playerDialogue.Text = attack.GetQuote();

			_activeDialogueTween = _playerDialogue.CreateTween();
			int textLength = _playerDialogue.Text.Length;
			float animationLength = textLength * .05f;
			PropertyTweener propTweener = _activeDialogueTween.TweenProperty
			(
				_playerDialogue.RTLabel,
				$"{Label.PropertyName.VisibleCharacters}",
				textLength,
				animationLength
			);
			propTweener.From(0);
			await ToSignal(_activeDialogueTween, Tween.SignalName.Finished);
		}

		public async Task PlayDialogAnimation(EnemyAttack attack)
		{
			_enemyDialogue.SetEntireVisibility(true);
			_playerDialogue.SetEntireVisibility(false);
			_enemyDialogue.RTLabel.VisibleCharacters = 0;
			_enemyDialogue.Text = attack.Quote;

			Tween tween = _enemyDialogue.CreateTween();
			int textLength = _enemyDialogue.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty
			(
				_enemyDialogue.RTLabel,
				$"{Label.PropertyName.VisibleCharacters}",
				textLength,
				.05f * textLength
			);
			propTweener.From(0);
			await ToSignal(propTweener, Tween.SignalName.Finished);
		}

		public void SkipDialogueAnimation(VariableDialogueControl variableDialogue)
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

		public void EnableInput()
		{
			_playerDialogue.MouseFilter = Control.MouseFilterEnum.Pass;
			_playerDialogue.RTLabel.AppendText("\n[right][img=40]res://Assets/UI/Icons/Arrow_2.png[/img][/right]");
			_playerDialogue.RTLabel.VisibleCharacters = -1;
		}

		public void OnSpeechBubblePressed()
		{
			PlayerTurnComplete?.Invoke();
		}

		public async Task UpdateConversationInterestMax()
		{
			if (_conversationInterestDelta == 0)
			{
				_conversationInterestMaxChange.Text = "";
				return;
			}
			else
			{
				_conversationInterestMaxChange.Text = $"{_conversationInterestDelta}";
			}

			//Fade-In ConversationInterestMaxChange
			Tween tween = _conversationInterestMaxChange.CreateTween();
			tween.TweenProperty
			(
				_conversationInterestMaxChange,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 255),
				1.0f
			);

			// Animate change to new Value
			int startValue = _conversationInterestValue.Text.ToInt();
			tween.TweenMethod(
				Callable.From<int>(_conversationInterestValue.SetLabelText),
				startValue, startValue + _conversationInterestDelta, 1.0f);
			int maxStartValue = _conversationInterestMax.Text.ToInt();
			tween.TweenMethod
			(
				Callable.From<int>(_conversationInterestMax.SetLabelText),
				maxStartValue,
				maxStartValue + _conversationInterestDelta,
				1.0f
			);

			//Fade-Out ConversationInterestMaxChange
			PropertyTweener propertyTweener = tween.TweenProperty
			(
				_conversationInterestMaxChange,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 0),
				1.0f
			);
			await ToSignal(propertyTweener, Tween.SignalName.Finished);
			_conversationInterestDelta = 0;
		}

		public async Task PlayAnimationsForAttack(PlayerAttack playerAttack, int bonusDamage)
		{
			_conversationInterestDamage.Text = $"-{playerAttack.ConversationInterestDamage + bonusDamage}";
			Tween tween = _conversationInterestDamage.CreateTween();
			tween.TweenProperty
			(
				_conversationInterestDamage,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 255),
				1.0f
			);

			// Animate change to new value
			int startValue = _conversationInterestValue.Text.ToInt();
			MethodTweener methodTween = tween.TweenMethod
			(
				Callable.From<int>(_conversationInterestValue.SetLabelText),
					startValue,
					startValue - playerAttack.ConversationInterestDamage - bonusDamage,
					1.0f
			).SetDelay(2.0f);
			await ToSignal(methodTween, Tween.SignalName.Finished);
			startValue = _conversationInterestValue.Text.ToInt();

			// Fade-Out label
			tween?.Kill();
			tween = _conversationInterestDamage.CreateTween();
			PropertyTweener propTweener = tween.TweenProperty
			(
				_conversationInterestDamage,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 0),
				1.0f
			);
			await ToSignal(propTweener, Tween.SignalName.Finished);
		}

		public async Task PlayAnimationsForAttack(EnemyAttack enemyAttack)
		{
			_mentalCapacityDamage.Text = $"-{enemyAttack.MentalCapacityDamage}";

			Tween tween = _mentalCapacityDamage.CreateTween();

			// Fade-In Damage label
			tween.TweenProperty
			(
				_mentalCapacityDamage,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 255),
				1.0f
			);

			// Animate change to new value
			int startValue = _mentalCapacityValue.Text.ToInt();
			tween.TweenMethod
			(
				Callable.From<int>(_mentalCapacityValue.SetLabelText),
				startValue,
				startValue - enemyAttack.MentalCapacityDamage,
				1.0f
			).SetDelay(2.0f);

			// Fade-Out label
			tween.TweenProperty
			(
				_mentalCapacityDamage,
				$"{Control.PropertyName.SelfModulate}",
				Color.Color8(255, 255, 255, 0),
				1.0f
			);

			// Animate Social Battery Progress
			Tween batteryTween = _socialBatteryProgress.CreateTween();
			PropertyTweener propTweener = batteryTween.TweenProperty
			(
				_socialBatteryProgress,
				$"{TextureProgressBar.PropertyName.Value}",
				_socialBatteryProgress.Value + enemyAttack.SocialBatteryChange,
				1.0f
			);
			await ToSignal(propTweener, Tween.SignalName.Finished);
		}

		public void UpdateTopic(TopicName topicName)
		{
			if (topicName != TopicName.None)
			{
				_currentTopic.Text = topicName.ToString();
			}
		}

		public void UpdateConversationInterestModifiers(int valueThroughAnnoyance, int valueThroughEnthusiasm)
		{
			_currentMaxCIChanges.Text =
			$"[font=res://Assets/Fonts/Lobster-Regular.ttf]+{valueThroughEnthusiasm}[/font] from Enthusiasm\n[font=res://Assets/Fonts/Lobster-Regular.ttf]{valueThroughAnnoyance}[/font] from Annoyance";
		}

		public void UpdateConversationInterest(EnthusiasmData data, TopicName topicName)
		{
			_conversationInterestDelta += data.ConversationInterestModDelta;
		}

		public void OnAnnoyanceChanged(AnnoyanceData data)
		{
			_conversationInterestDelta += data.ConversationInterestModDelta;
			_socialBatteryProgress.Value += data.SocialBatteryDamage;
		}

		public override void _ExitTree()
		{
			ConversationTopic.EnthusiasmChangedForTopic -= UpdateConversationInterest;
			AnnoyanceLevel.Changed -= OnAnnoyanceChanged;
			EncounterCharacter.TopicChanged -= UpdateTopic;
			base._ExitTree();
		}
	}
}
