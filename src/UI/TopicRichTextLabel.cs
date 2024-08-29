using Godot;
using System;
using tee;

public partial class TopicRichTextLabel : RichTextLabel
{
	private Preference _preference = Preference.Unknown;
	private TopicName _topicName = TopicName.None;
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
		get; set;
	}
	public Preference Preference{
		get{return _preference;}
		set{_preference = value;}
	}
}
