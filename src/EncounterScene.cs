using Godot;
using System;
using Godot.Collections;

namespace tee
{
	public delegate void EncounterSceneHandler();
	public partial class EncounterScene : Control
	{
		public event EncounterSceneHandler SetupCompleted;
		public static event EncounterSceneHandler PlayerTurnAnimationComplete;
		public static event EncounterSceneHandler EnemyTurnAnimationComplete;
		[Export] private Sprite2D _playerSprite;
		[Export] private Sprite2D _enemySprite;
		[Export] private Array<AttackButton> _attackButtons;
		[Export] private AnimationPlayer _animationPlayer;
		[Export] private Label _dialogueLine;
		[Export] private Container _dialogueBox;
		[Export] private TextureProgressBar _socialBatteryProgress;
		[Export] private Button _leaveButton;
		private AttackButton _currentlySelectedButton;
		private EnemyData _currentEnemy;
		public EnemyData CurrentEnemy
		{
			get { return _currentEnemy; }
			set { _currentEnemy = value; }
		}
		public Button LeaveButton
		{
			get { return _leaveButton; }
		}
		public Array<AttackButton> AttackButtons
		{
			get { return _attackButtons; }
		}

		public override void _Ready()
		{
			SceneManager.EncounterScene = this;
			AttackButton.OnButtonPressed += SetCurrentlySelectedButton;
		}

		public void SetupScene(EnemyData enemyData)
		{
			_currentEnemy = enemyData;
			_enemySprite.Texture = enemyData.Sprite;
			_enemySprite.Hframes = 6;
			_enemySprite.Vframes = 3;
			_dialogueLine.Text = "";
			_socialBatteryProgress.Value = GameManager.PlayerData.SocialBattery;
			SetupCompleted?.Invoke();
		}

		private void SetCurrentlySelectedButton(AttackButton button)
		{
			_currentlySelectedButton = button;
		}

		public void SetupSelectedButton(PlayerAttack attack)
		{
			_currentlySelectedButton.SetupButton(attack);
		}

		public void PlayCombatAnimation(PlayerAttack attack)
		{
			_dialogueLine.Text = attack.Dialogue;
			Tween tween = _dialogueLine.CreateTween();
			int textLength = _dialogueLine.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_dialogueLine, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
			propTweener.From(0);
			tween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
			//_animationPlayer.Play("PlayerTurn");
		}

		public void PlayCombatAnimation(EnemyAttack attack)
		{
			_dialogueLine.Text = attack.Dialogue;
			Tween tween = _dialogueLine.CreateTween();
			int textLength = _dialogueLine.Text.Length;
			PropertyTweener propTweener = tween.TweenProperty(
				_dialogueLine, $"{Label.PropertyName.VisibleCharacters}", textLength, .05f * textLength);
			propTweener.From(0);
			tween.TweenCallback(Callable.From(() => PlayAnimationsForAttack(attack)));
			//_animationPlayer.Play("PlayerTurn");
		}

		private void PlayAnimationsForAttack(PlayerAttack playerAttack)
		{

			if (playerAttack.InterestChange < 0)
			{
				// Play Negative Feedback Animation
			}
			else if (playerAttack.InterestChange > 0)
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

		}
	}
}
