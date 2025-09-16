using Godot;
using Godot.Collections;

namespace tee
{
    public delegate void TopicEventHandler(TopicName topicName);
    public abstract partial class EncounterCharacter : GodotObject
    {
        public static event TopicEventHandler TopicChanged;
        private TopicName _currentTopicName = TopicName.None;
        protected TopicName _lastTopicName = TopicName.None;
        public TopicName CurrentTopicName
        {
            get { return _currentTopicName; }
            set
            {
                _currentTopicName = value;
                TopicChanged?.Invoke(value);
            }
        }

        public TopicName LastTopicName
        {
            get;set;
        }

    }
}
