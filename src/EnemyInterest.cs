using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using tee;

public partial class EnemyInterest : GodotObject
{
	private TopicName _name;
	private int _propability = 5;
	private int _enthusiasmStacks;
	private int _annoyanceStacks;
	public TopicName Name
	{
		get { return _name; }
	}
	public int Propability
	{
		get; set;
	}

	[Range(0, 5, ErrorMessage = "Value for {EnthusiasmStacks} must be between {0} and {5}.")]
	public int EnthusiasmStacks
	{
		get; set;
	}

	[Range(0, 5, ErrorMessage = "Value for {AnnoyanceStacks} must be between {0} and {5}.")]
	public int AnnoyanceStacks
	{
		get; set;
	}

	public EnemyInterest(TopicName name)
	{
		_name = name;
	}

	public void IncreaseEnthusiasm()
	{
		_enthusiasmStacks++;
	}
}
