using Godot;
using System;
using tee;

public partial class EncounterStartScreen : Control
{
	[Export] private Label _enemyName;
	[Export] private TextureRect _enemyTexture;
	[Export] private AnimationPlayer _animationPlayer;

    public override void _EnterTree()
    {
        _enemyName.Text = GameManager.CurrentEnemy.DisplayName;
		_enemyTexture.Texture = GameManager.CurrentEnemy.Texture;
		_animationPlayer.Play("FlyIn");
    }


}
