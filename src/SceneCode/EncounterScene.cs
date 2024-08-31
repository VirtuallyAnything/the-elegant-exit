using System;
using System.Reflection.Metadata.Ecma335;
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
		private bool _attackAnimationsFinished = true;
		private bool _dialogueAnimationFinished = true;
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
			EnthusiasmLevel.ConversationInterestChanged += UpdateConversationInterest;
			AnnoyanceLevel.ConversationInterestChanged += UpdateConversationInterest;
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
			valueTween.Parallel().TweenProperty(
				_socialBatteryProgress, $"{TextureProgressBar.PropertyName.Value}", _socialBatteryProgress.Value - socialBatteryDifference, 1.0f);
			valueTween.Parallel().TweenMethod(Callable.From<int>(_mentalCapacityValue.SetLabelText), 0.0f, socialBatteryDifference, 1.0f);
			_animationPlayer.SpeedScale = -1;
			valueTween.TweenCallback(Callable.From(() => _animationPlayer.Play("SocialBatterySubtract", -1, 1, true)));
			await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}

		public async Task PlayDialogAnimation(PlayerAttack attack)
		{
			_dialogueAnimationFinished = false;
			_playerDialogue.SetEntireVisibility(true);
			_enemyDialogue.SetEntireVisibility(false);

			_playerDialogue.Text = attack.GetQuote();
			//_dialogueLine.Modulate = _playerDialogueColor;

			_activeDialogueTween = _playerDialogue.CreateTween();
			int textLength = _playerDialogue.Text.Length;
			float animationLength = textLength * .05f;
			PropertyTweener propTweener = _activeDialogueTween.TweenProperty(
				_playerDialogue.RTLabel, $"{Label.PropertyName.VisibleCharacters}", textLength, animationLength);
			propTweener.From(0);
			_activeDialogueTween.TweenCallback(Callable.From(() => _attackAnimationsFinished = true));
			//_activeDialogueTween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
			await ToSignal(_activeDialogueTween, Tween.SignalName.Finished);
			_dialogueAnimationFinished = true;
		}

		public async Task PlayDialogAnimation(EnemyAttack attack)
		{
			_enemyDialogue.SetEntireVisibility(true);
			_playerDialogue.SetEntireVisibility(false);
			_enemyDialogue.Text = attack.Quote;

			Tween tween = _enemyDialogue.CreateTween();
			int textLength = _enemyDialogue.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_enemyDialogue.RTLabel, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
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

		public void OnSpeechBubblePressed()
		{
			if (_attackAnimationsFinished && _dialogueAnimationFinished)
			{
				PlayerTurnComplete?.Invoke();
			}
		}

		public async Task UpdateConversationInterestMax()
		{
			_attackAnimationsFinished = false;
			if (_conversationInterestDelta == 0)
			{
				_conversationInterestMaxChange.Text = "";
				_attackAnimationsFinished = true;
				return;
			}
			else
			{
				_conversationInterestMaxChange.Text = $"{_conversationInterestDelta}";
			}

			//Fade-In ConversationInterestMaxChange
			Tween tween = _conversationInterestMaxChange.CreateTween();
			tween.TweenProperty(
			_conversationInterestMaxChange, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 255), 1f);
			// Animate change to new Value
			int startValue = _conversationInterestValue.Text.ToInt();
			tween.TweenMethod(Callable.From<int>(_conversationInterestValue.SetLabelText), startValue, startValue + _conversationInterestDelta, 1.0f);
			int maxStartValue = _conversationInterestMax.Text.ToInt();
			tween.TweenMethod(Callable.From<int>(_conversationInterestMax.SetLabelText), maxStartValue, maxStartValue + _conversationInterestDelta, 1.0f);
			//Fade-Out ConversationInterestMaxChange
			PropertyTweener propertyTweener = tween.TweenProperty(
						_conversationInterestMaxChange, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 0), 1f);
			await ToSignal(propertyTweener, Tween.SignalName.Finished);
			_conversationInterestDelta = 0;
			_attackAnimationsFinished = true;
		}

		public async Task PlayAnimationsForAttack(PlayerAttack playerAttack, int bonusDamage)
		{
			_attackAnimationsFinished = false;
			// Add Icon to conversationInterestDamage.Text depending on
			_conversationInterestDamage.Text = $"-{playerAttack.ConversationInterestDamage + bonusDamage}";
			Tween tween = _conversationInterestDamage.CreateTween();
			tween.TweenProperty(
			_conversationInterestDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 255), 1f);
			// Animate change to new value
			int startValue = _conversationInterestValue.Text.ToInt();
			MethodTweener methodTween = tween.TweenMethod(Callable.From<int>(_conversationInterestValue.SetLabelText), startValue, startValue - playerAttack.ConversationInterestDamage - bonusDamage, 1.0f).SetDelay(2f);
			await ToSignal(methodTween, Tween.SignalName.Finished);
			startValue = _conversationInterestValue.Text.ToInt();

			// Fade-Out label
			tween?.Kill();
			tween = _conversationInterestDamage.CreateTween();
			PropertyTweener propTweener = tween.TweenProperty(
				_conversationInterestDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 0), 1f);
			await ToSignal(propTweener, Tween.SignalName.Finished);
			_attackAnimationsFinished = true;	
		}

		public async Task PlayAnimationsForAttack(EnemyAttack enemyAttack)
		{
			_mentalCapacityDamage.Text = $"-{enemyAttack.MentalCapacityDamage}";

			Tween tween = _mentalCapacityDamage.CreateTween();
			// Fade-In Damage label
			tween.TweenProperty(
				_mentalCapacityDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 255), 1f);
			// Animate change to new value
			int startValue = _mentalCapacityValue.Text.ToInt();
			tween.TweenMethod(Callable.From<int>(_mentalCapacityValue.SetLabelText), startValue, startValue - enemyAttack.MentalCapacityDamage, 1.0f).SetDelay(2f);
			// Fade-Out label
			tween.TweenProperty(
				_mentalCapacityDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 0), 1f);

			// Animate Social Battery Progress
			Tween batteryTween = _socialBatteryProgress.CreateTween();
			PropertyTweener propTweener = batteryTween.TweenProperty(
				_socialBatteryProgress, $"{TextureProgressBar.PropertyName.Value}", _socialBatteryProgress.Value + enemyAttack.SocialBatteryChange, 1f);
			await ToSignal(propTweener, Tween.SignalName.Finished);			
		}

		public void UpdateAnnoyance(AnnoyanceLevel annoyanceLevel)
		{
			_annoyanceDisplay.DisplayAnnoyance(annoyanceLevel);
		}

		public void UpdateTopic(TopicName topicName)
		{
			_currentTopic.Text = topicName.ToString();
		}

		public void UpdateConversationInterestModifiers(int valueThroughAnnoyance, int valueThroughEnthusiasm)
		{
			_currentMaxCIChanges.Text = $"[font=res://Assets/Fonts/Lobster-Regular.ttf]+{valueThroughEnthusiasm}[/font] from Enthusiasm\n[font=res://Assets/Fonts/Lobster-Regular.ttf]{valueThroughAnnoyance}[/font] from Annoyance";
		}

		public void UpdateConversationInterest(int value, GodotObject godotObject)
		{
			_conversationInterestDelta += value;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}
