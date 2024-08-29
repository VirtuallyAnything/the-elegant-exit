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
		public static event EncounterSceneHandler EnemyTurnComplete;

		[Export] private TextureRect _playerSprite;
		[Export] private Color _playerDialogueColor;
		[Export] private VariableDialogueControl _playerDialogue;

		[Export] private TextureRect _enemySprite;
		[Export] private Color _enemyDialogueColor;
		[Export] private VariableDialogueControl _enemyDialogue;
		[Export] private Label _enemyName;
		private EnemyData _currentEnemy;

		private Label _labelToTweak;
		private Tween _activeDialogueTween;
		private bool _dialogueAnimationFinished = true;
		[Export] private AttackCardContainer _attackCardContainer;
		[Export] private AnimationPlayer _animationPlayer;

		[Export] private Label _annoyanceValue;

		[Export] private Label _mentalCapacityMax;
		[Export] private Label _mentalCapacityValue;
		[Export] private Label _mentalCapacityDamage;

		[Export] private Label _conversationInterestMax;
		[Export] private Label _conversationInterestValue;
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
				_socialBatteryProgress, $"{TextureProgressBar.PropertyName.Value}", _socialBatteryProgress.Value - socialBatteryDifference, 2f);
			_labelToTweak = _mentalCapacityValue;
			valueTween.Parallel().TweenMethod(Callable.From<int>(SetLabelText), 0.0f, socialBatteryDifference, 1.0f);
			_animationPlayer.SpeedScale = -1;
			valueTween.TweenCallback(Callable.From(() => _animationPlayer.Play("SocialBatterySubtract", -1, 1, true)));
			await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		}

		public void SetLabelText(int toNumber)
		{
			_labelToTweak.Text = $"{toNumber}";
		}

		public void PlayDialogAnimation(PlayerAttack attack)
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

		public void PlayDialogAnimation(EnemyAttack attack)
		{
			_enemyDialogue.SetEntireVisibility(true);
			_playerDialogue.SetEntireVisibility(false);
			_enemyDialogue.Text = attack.Quote;

			Tween tween = _enemyDialogue.CreateTween();
			int textLength = _enemyDialogue.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_enemyDialogue.RTLabel, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
			propTweener.From(0);
			//tween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
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
			if (_dialogueAnimationFinished)
			{
				PlayerTurnComplete?.Invoke();
			}
		}

		private void PlayAnimationsForAttack(PlayerAttack playerAttack)
		{
			_conversationInterestDamage.Text = $"-{playerAttack.ConversationInterestDamage}";
			Tween tween = _conversationInterestDamage.CreateTween();
			tween.TweenProperty(
			_conversationInterestDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 255), 1f);
			// Animate change to new value
			_labelToTweak = _conversationInterestValue;
			int startValue = _conversationInterestValue.Text.ToInt();
			tween.TweenMethod(Callable.From<int>(SetLabelText), startValue, startValue - playerAttack.ConversationInterestDamage, 1.0f).SetDelay(2f);
			// Fade-Out label
			tween.TweenProperty(
				_conversationInterestDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 0), 1f);

			//PlayerTurnAnimationComplete?.Invoke();

			//Let the player click again when all the animations have finished
		}

		public void PlayAnimationsForAttack(EnemyAttack enemyAttack)
		{
			_mentalCapacityDamage.Text = $"-{enemyAttack.MentalCapacityDamage}";

			Tween tween = _mentalCapacityDamage.CreateTween();
			// Fade-In Damage label
			tween.TweenProperty(
				_mentalCapacityDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 255), 1f);
			// Animate change to new value
			_labelToTweak = _mentalCapacityValue;
			int startValue = _mentalCapacityValue.Text.ToInt();
			tween.TweenMethod(Callable.From<int>(SetLabelText), startValue, startValue - enemyAttack.MentalCapacityDamage, 1.0f).SetDelay(2f);
			// Fade-Out label
			tween.TweenProperty(
				_mentalCapacityDamage, $"{Control.PropertyName.SelfModulate}", Color.Color8(255, 255, 255, 0), 1f);
			// Animate Social Battery Progress
			Tween batteryTween = _socialBatteryProgress.CreateTween();
			PropertyTweener propTweener = batteryTween.TweenProperty(
				_socialBatteryProgress, $"{TextureProgressBar.PropertyName.Value}", _socialBatteryProgress.Value + enemyAttack.SocialBatteryChange, 1f);
			batteryTween.SetParallel(true);
			tween.TweenCallback(Callable.From(() => EnemyTurnComplete?.Invoke()));
		}

		public void UpdateAnnoyance(int annoyanceNew)
		{
			_annoyanceValue.Text = annoyanceNew.ToRomanNumerals();
		}

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}
