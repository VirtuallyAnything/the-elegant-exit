using Godot;
using System;
using tee;

public partial class TopicRichTextLabel : RichTextLabel
{
	private Preference _preference = Preference.Unknown;
	private TopicName _topicName = TopicName.None;
	private string _iconPath = "[img=30]res://Assets/UI/Icons/UnknownIcon.png[/img]";
	[Export] 
	public TopicName TopicName{
		get{return _topicName;}
		set{_topicName = value;}
	}
	public string EnthusiasmLevel{
		get; set;
	}
	public int EnthusiasmNumber{
		get; set;
	}
	public string IconPath{
		get{return _iconPath;}
		set{_iconPath = value;}
	}
	public Preference Preference{
		get{return _preference;}
		set{_preference = value;}
	}
}
