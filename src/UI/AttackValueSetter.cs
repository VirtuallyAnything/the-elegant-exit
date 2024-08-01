using Godot;
using System;
using tee;

public partial class AttackValueSetter : Container
{
	[Export] private Label _socialStandingChange;
	[Export] private Label _socialBatteryChange;
	[Export] private Label _conversationInterestChange;
	[Export] private Label _mentalCapacityChange;

	public void SetLabels(PlayerAttack attack){
		_socialStandingChange.Text = $"{attack.SocialStandingChangeLike}/{attack.SocialStandingChangeDislike}";
		_socialBatteryChange.Text = $"{attack.SocialBatteryChangeLike}/{attack.SocialBatteryChangeDislike}";
		_conversationInterestChange.Text = $"{attack.ConversationInterestChangeLike}/{attack.ConversationInterestChangeDislike}";
		_mentalCapacityChange.Text = $"{attack.MentalCapacityChangeLike}/{attack.MentalCapacityChangeDislike}";
	}
}
