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
		get; set;
	}

	public ConversationTopic(TopicName name)
	{
		_name = name;
	}

	public void IncreaseEnthusiasm()
	{
		_enthusiasmLevel.Increase();
	}

	public void DecreaseEnthusiasm(){
		_enthusiasmLevel.Decrease();
	}

	public int GetCurrentEnthusiasmLevel(){
		return _enthusiasmLevel.CurrentEnthusiasm;
	}
}
