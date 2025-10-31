using Godot;

namespace tee
{
	public partial class EncounterStartScene : TempScene
	{
		[Export] private Label _enemyName;
		[Export] private TextureRect _enemyTexture;
		[Export] private Label _enemyTitle;
		[Export] private AnimationPlayer _animationPlayer;

		public override void _EnterTree()
		{
			base._EnterTree();
			_enemyName.Text = GameManager.CurrentEnemy.DisplayName;
			_enemyTexture.Texture = GameManager.CurrentEnemy.Texture;
			_enemyTitle.Text = GameManager.CurrentEnemy.Title;
			_animationPlayer.Play("FlyIn");
		}
	}
}
