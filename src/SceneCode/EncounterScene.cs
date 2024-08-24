using Godot;

namespace tee
{
	public delegate void EncounterSceneHandler();
	public partial class EncounterScene : Scene
	{
		public event EncounterSceneHandler SetupCompleted;
		public static event EncounterSceneHandler PlayerTurnAnimationComplete;
		public static event EncounterSceneHandler EnemyTurnAnimationComplete;
		
		[Export] private Sprite2D _playerSprite;
		[Export] private Color _playerDialogueColor;

		[Export] private Sprite2D _enemySprite;
		[Export] private Color _enemyDialogueColor;
		[Export] private Label _enemyName;
		private EnemyData _currentEnemy;

		[Export] private AttackCardContainer _attackCardContainer;
		[Export] private AnimationPlayer _animationPlayer;
		[Export] private RichTextLabel _dialogueLine;
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
			_dialogueLine.Text = "";
			_dialogueLine.Modulate = _enemyDialogueColor;
			_socialBatteryProgress.Value = GameManager.SocialBattery;
			SetupCompleted?.Invoke();
		}

		public async void PlayCombatAnimation(PlayerAttack attack)
		{
			_dialogueLine.Text = attack.GetQuote();
			_dialogueLine.Modulate = _playerDialogueColor;
			Tween tween = _dialogueLine.CreateTween();
			int textLength = _dialogueLine.Text.Length;
			float animationLength = textLength * .05f;
			PropertyTweener propTweener = tween.TweenProperty(
				_dialogueLine, $"{Label.PropertyName.VisibleCharacters}", textLength, animationLength);
			propTweener.From(0);
			await ToSignal(GetTree().CreateTimer(animationLength * 2), SceneTreeTimer.SignalName.Timeout);
			PlayAnimationsForAttack(attack);
		}

		public void PlayCombatAnimation(EnemyAttack attack)
		{
			_dialogueLine.Text = attack.Quote;
			_dialogueLine.Modulate = _enemyDialogueColor;
			Tween tween = _dialogueLine.CreateTween();
			int textLength = _dialogueLine.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_dialogueLine, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
			propTweener.From(0);
			tween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
		}

		private void PlayAnimationsForAttack(PlayerAttack playerAttack)
		{

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
			PlayerTurnAnimationComplete?.Invoke();
		}

		private void PlayAnimationsForAttack(EnemyAttack enemyAttack)
		{
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
			EnemyTurnAnimationComplete?.Invoke();
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
    }
}
