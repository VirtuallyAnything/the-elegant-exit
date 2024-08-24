using Godot;
using tee;

public partial class ConversationTopic : GodotObject
{
	private TopicName _name;
	private int _weight = 5;
	private EnthusiasmLevel _enthusiasmLevel = new();
	public TopicName Name
	{
		get { return _name; }
	}
	public int Weight
	{
		get{ return _weight;}
		set{ _weight = value;}
	}

	public ConversationTopic(TopicName name)
	{
		_name = name;
	}

	public void IncreaseEnthusiasm()
	{
		_enthusiasmLevel.Increase();
		Weight += 5;
		GD.Print($"Enthusiasm Increased. ConversationTopic {Name} now has Enthusiasm Level {_enthusiasmLevel.CurrentEnthusiasm} and a Weight of {Weight}.");
	}

	public void DecreaseEnthusiasm(){
		_enthusiasmLevel.Decrease();
		Weight -= 5;
		GD.Print($"Enthusiasm Decreased. ConversationTopic {Name} now has Enthusiasm Level {_enthusiasmLevel.CurrentEnthusiasm} and a Weight of {Weight}.");
	}

	public int GetCurrentEnthusiasmLevel(){
		return _enthusiasmLevel.CurrentEnthusiasm;
	}
}
