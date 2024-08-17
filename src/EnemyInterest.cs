using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using tee;

public partial class EnemyInterest : GodotObject
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

	public EnemyInterest(TopicName name)
	{
		_name = name;
	}

	public void IncreaseEnthusiasm()
	{
		_enthusiasmLevel.Increase();
	}
}
