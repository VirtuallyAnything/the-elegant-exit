using Godot;
using System;

namespace tee
{
    [GlobalClass]
    public partial class TopicDialogue : Resource
    {
        [Export] private TopicName _topicName;
        [Export] private string _text;
        public TopicName TopicName{
            get{return _topicName;}
        }
        public string Text{
            get{return _text;}
        }
    }
}
