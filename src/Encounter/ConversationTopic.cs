using Godot;
namespace tee
{
	public delegate void TopicEnthusiasmHandler(EnthusiasmData enthusiasmData, TopicName topicName);
	public partial class ConversationTopic(TopicName name) : GodotObject
	{
		public static event TopicEnthusiasmHandler EnthusiasmChangedForTopic;
		private TopicName _name = name;
		private int _weight = 5;
		private EnthusiasmLevel _enthusiasmLevel = new();
		public TopicName Name
		{
			get { return _name; }
		}
		public int Weight
		{
			get { return _weight; }
			set
			{
				if (value > 35)
				{
					_weight = 35;
				}
				else if (value < 0)
				{
					_weight = 0;
				}
				else
				{
					_weight = value;
				}
			}
		}

		public void IncreaseEnthusiasm()
		{
			_enthusiasmLevel.Increase();
			if (Weight != 0)
			{
				Weight += 5;
			}
			EnthusiasmChangedForTopic?.Invoke(_enthusiasmLevel.PackageCurrentData(), Name);
			GD.Print($"Enthusiasm Increased. ConversationTopic {Name} now has Enthusiasm Level {_enthusiasmLevel.CurrentEnthusiasm} and a Weight of {Weight}.");
		}

		public void DecreaseEnthusiasm()
		{
			_enthusiasmLevel.Decrease();
			if (Weight != 0)
			{
				Weight -= 5;
			}
			EnthusiasmChangedForTopic?.Invoke(_enthusiasmLevel.PackageCurrentData(), Name);
			GD.Print($"Enthusiasm Decreased. ConversationTopic {Name} now has Enthusiasm Level {_enthusiasmLevel.CurrentEnthusiasm} and a Weight of {Weight}.");
		}

		public int GetCurrentEnthusiasmLevel()
		{
			return _enthusiasmLevel.CurrentEnthusiasm;
		}

		public int GetCurrentSocialStanding()
		{
			return _enthusiasmLevel.SocialStandingChange;
		}

        public override void _Notification(int what)
        {
            if(what == NotificationPredelete)
            {
				_enthusiasmLevel.Free();
            }
        }


	}
}