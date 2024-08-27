using Godot;
using Godot.Collections;
using System;
using System.Linq;
using tee;

public partial class PreferenceDisplay : Control
{
	[Export] private Array<TopicRichTextLabel> _topicLabels;
	[Export] private Texture2D _likeIcon, _dislikeIcon, _unknownIcon;

	public override void _Ready()
	{
		foreach(TopicRichTextLabel topicLabel in _topicLabels){
			topicLabel.Text = 
			$"[hint='Preference: {topicLabel.Preference}'][center]{topicLabel.TopicName}[img]{_unknownIcon.ResourcePath}[/img][/center]";
		}
	}

	public void UpdatePreference(TopicName topicName, Preference preference, int enthusiasmLevel){
		foreach(TopicRichTextLabel label in _topicLabels){
			if(label.TopicName == topicName){
				label.Preference = preference;
				string iconPath = "[img]";
				switch(preference){
					case Preference.Like:
					iconPath += _likeIcon.ResourcePath + "[/img]";
					break;
					case Preference.Dislike:
					iconPath += _dislikeIcon.ResourcePath + "[/img]";
					break;
					default:
					iconPath = "";
					break;
				}
				
				label.Text = 
			$"[hint='Preference: {preference}" + $"\nEnthusiasm: {enthusiasmLevel}" + $"'][center]{topicName}" + iconPath + "[/center]";
			}
		}
	}

	public void UpdateEnthusiasm(TopicName topicName, int enthusiasmLevel){
		foreach(TopicRichTextLabel label in _topicLabels){
			if(label.TopicName == topicName){				
				int index = label.Text.Find("Enthusiasm"); 
				//label.Text.Replace()
			}
		}
	}

}
