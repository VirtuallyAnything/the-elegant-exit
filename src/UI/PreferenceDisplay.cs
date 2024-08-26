using Godot;
using Godot.Collections;
using System;
using System.Linq;
using tee;

public partial class PreferenceDisplay : Control
{
	[Export] private Array<RichTextLabel> _topicLabels;
	[Export] private Texture2D _likeIcon, _dislikeIcon, _unknownIcon;
	private Dictionary<TopicName, Preference> _topicPreferences = new(){
		{TopicName.Art, Preference.Unknown},
		{TopicName.Economy, Preference.Unknown},
		{TopicName.Gossip, Preference.Unknown},
		{TopicName.Lifestyle, Preference.Unknown},
		{TopicName.Politics, Preference.Unknown},
		{TopicName.Sport, Preference.Unknown}
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Array <TopicName> keys = (Array<TopicName>)_topicPreferences.Keys;
		for(int i = 0; i < _topicLabels.Count; i++){
			_topicLabels[i].BbcodeEnabled = true;
			TopicName topic = keys[i];
			_topicLabels[i].Text = 
			$"[hint='Preference: {Preference.Unknown}'][center]{topic}[img]{_unknownIcon.ResourcePath}[/img][/center]";
		}
	}

	public void UpdatePreference(TopicName topicName, Preference preference){
		foreach(RichTextLabel label in _topicLabels){
			if(label.Text.Contains($"{topicName}")){
				string iconPath = "[img]";
				switch(preference){
					case Preference.Like:
					iconPath += _likeIcon.ResourcePath;
					break;
					case Preference.Dislike:
					iconPath += _dislikeIcon.ResourcePath;
					break;
				}
				iconPath += "[/img]";
				label.Text = 
			$"[hint='Preference: {preference}'][center]{topicName}" + iconPath + "[/center]";
			}
		}
	}

	public void UpdateEnthusiasm(TopicName topicName, int to){
		
	}
}
