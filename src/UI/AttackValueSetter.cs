using Godot;
using System;
using tee;

public partial class AttackValueSetter : Container
{
	[Export] private Label _socialBatteryChange;
	[Export] private Label _conversationInterestChange;
	[Export] private Label _mentalCapacityChange;

	public void SetLabels(PlayerAttack attack){
		_socialBatteryChange.Text = $"{attack.SocialBatteryChange}";
		_conversationInterestChange.Text = $"{attack.ConversationInterestChange}";
	}
}
