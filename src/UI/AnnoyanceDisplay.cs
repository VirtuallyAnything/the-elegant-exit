using Godot;
using System;
using tee;

public partial class AnnoyanceDisplay : Control
{
	[Export] private Control _arrowLeft;
	[Export] private Control _arrowRight;
	[Export] private Control _arrowRightCIPlusIcon;
	[Export] private Control _arrowRightSBPlusIcon;
	[Export] private Label _conversationInterestNext;
	[Export] private Label _conversationInterestLast;
	[Export] private Label _socialBatteryNext;
	[Export] private Label _maxLevelText;
	[Export] private Label _annoyanceValue;

	public override void _Ready()
	{
		_arrowLeft.Modulate = Color.Color8(255, 255, 255, 0);
		AnnoyanceLevel.Changed += Update;
		Update(new AnnoyanceData());
	}

	public void Update(AnnoyanceData data)
	{
		int currentLevel = data.CurrentAnnoyance;
		_annoyanceValue.Text = currentLevel.ToRomanNumerals();
		if (currentLevel > 0)
		{
			_arrowLeft.Modulate = Color.Color8(255, 255, 255, 255);
			_conversationInterestLast.Text = $"{AnnoyanceLevel.GetCIDeltaForDecreaseTo(currentLevel - 1).Signed()}";
		}
		else
		{
			_arrowLeft.Modulate = Color.Color8(255, 255, 255, 0);
		}

		if (currentLevel == 4)
		{
			_arrowRightCIPlusIcon.Visible = false;
			_arrowRightSBPlusIcon.Visible = false;
			_maxLevelText.Visible = true;
		}
		else
		{
			_arrowRightCIPlusIcon.Visible = true;
			_arrowRightSBPlusIcon.Visible = true;
			_maxLevelText.Visible = false;
			_conversationInterestNext.Text = $"{AnnoyanceLevel.GetCIDeltaForIncreaseTo(currentLevel + 1)}";
			_socialBatteryNext.Text = $"{AnnoyanceLevel.GetSocialBatteryDamageForLevel(currentLevel + 1)}";
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		AnnoyanceLevel.Changed -= Update;
	}
}
