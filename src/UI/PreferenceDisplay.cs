using System.Linq;
using Godot;
using Godot.Collections;
using tee;

public partial class PreferenceDisplay : Control
{
	[Export] private Array<TopicRichTextLabel> _topicLabels;
	[Export] private Texture2D _likeIcon, _dislikeIcon, _neutralIcon, _unknownIcon;
	private string _specialInterestName;

	public override void _Ready()
	{
		foreach (TopicRichTextLabel topicLabel in _topicLabels)
		{
			topicLabel.Text =
			$"[hint='Preference: {topicLabel.Preference}'][center]{topicLabel.TopicName}[img=30]{_unknownIcon.ResourcePath}[/img][/center]";
		}
		CombatManager.PreferenceDiscovered += UpdatePreference;
		ConversationTopic.EnthusiasmChangedForTopic += UpdateEnthusiasm;
	}

	public void SetSpecialInterest(string specialInterestName)
	{
		_specialInterestName = specialInterestName;
		foreach (TopicRichTextLabel topicLabel in _topicLabels)
		{
			if (topicLabel.TopicName == TopicName.SpecialInterest)
			{
				topicLabel.Text =
				$"[hint='Preference: {topicLabel.Preference}'][center]{specialInterestName}[img=30]{_unknownIcon.ResourcePath}[/img][/center]";
			}
		}
	}

	public void UpdatePreference(TopicName topicName, Preference preference)
	{
		foreach (TopicRichTextLabel label in _topicLabels)
		{
			if (label.TopicName == topicName)
			{
				label.Preference = preference;
				string iconPath = "[img=30]";
				switch (preference)
				{
					case Preference.Like:
						iconPath += _likeIcon.ResourcePath + "[/img]";
						break;
					case Preference.Dislike:
						iconPath += _dislikeIcon.ResourcePath + "[/img]";
						break;
					case Preference.Neutral:
						iconPath += _neutralIcon.ResourcePath + "[/img]";
						break;
					default:
						iconPath = "";
						break;
				}
				label.IconPath = iconPath;

				label.Text =
			$"[hint='Preference: {preference}\nEnthusiasm: {label.EnthusiasmNumber}" + $"'][center]{topicName}" + iconPath + $"{label.EnthusiasmLevel}[/center]";
			}
		}
	}

	public void UpdateEnthusiasm(EnthusiasmData data, TopicName topicName)
	{
		foreach (TopicRichTextLabel label in _topicLabels)
		{
			if (label.TopicName == topicName)
			{
				label.EnthusiasmNumber = data.CurrentEnthusiasm;
				string enthusiasmRomanNumeral = "\n";
				enthusiasmRomanNumeral += data.CurrentEnthusiasm.ToRomanNumerals();
				label.EnthusiasmLevel = enthusiasmRomanNumeral;
				string displayName;
				if(topicName == TopicName.SpecialInterest)
                {
					displayName = _specialInterestName;
                }
                else
                {
					displayName = topicName.ToString();
                }
				label.Text = $"[hint='Preference: {label.Preference}\nEnthusiasm: {label.EnthusiasmNumber}" + $"'][center]{displayName}" + label.IconPath + enthusiasmRomanNumeral + "[/center]";
			}
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		CombatManager.PreferenceDiscovered -= UpdatePreference;
		ConversationTopic.EnthusiasmChangedForTopic -= UpdateEnthusiasm;
	}

}
