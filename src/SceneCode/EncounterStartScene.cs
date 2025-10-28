using System;
using System.Threading.Tasks;
using Godot;

namespace tee
{
	public partial class EncounterStartScene : TempScene
	{
		[Export] private Label _enemyName;
		[Export] private TextureRect _enemyTexture;
		[Export] private AnimationPlayer _animationPlayer;

		public override void _EnterTree()
		{
			base._EnterTree();
			_enemyName.Text = GameManager.CurrentEnemy.DisplayName;
			_enemyTexture.Texture = GameManager.CurrentEnemy.Texture;

			_animationPlayer.Play("FlyIn");
		}
	}
}
