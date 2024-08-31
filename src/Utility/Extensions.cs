using Godot;
using System;
using System.Collections.Generic;
using tee;

public static class Extensions : Object
{
	public static Vector2[] ToVertices(this Vector2 size, Vector2 origin, bool centerVerticesAroundOrigin = false)
	{
		Vector2[] vertices = new Vector2[4];
		if (centerVerticesAroundOrigin)
		{
			vertices[0] = new Vector2(origin.X + size.X / 2, origin.Y - size.Y / 2);
			vertices[1] = new Vector2(origin.X + size.X / 2, origin.Y + size.Y / 2);
			vertices[2] = new Vector2(origin.X - size.X / 2, origin.Y + size.Y / 2);
			vertices[3] = new Vector2(origin.X - size.X / 2, origin.Y - size.Y / 2);
		}
		else
		{
			vertices[0] = origin;
			vertices[1] = new Vector2(origin.X, origin.Y + size.Y);
			vertices[2] = new Vector2(origin.X + size.X, origin.Y + size.Y);
			vertices[3] = new Vector2(origin.X + size.X, origin.Y);
		}
		return vertices;
	}

	public static Godot.Collections.Array<ConversationTopic> ToConversationTopics(this Godot.Collections.Array<TopicName> topicNames)
	{
		Godot.Collections.Array<ConversationTopic> conversationTopics = new();
		foreach (TopicName topic in topicNames)
		{
			ConversationTopic conversationTopic = new(topic);
			conversationTopics.Add(conversationTopic);
		}
		return conversationTopics;
	}

	// Code by Nevermind on https://softwareengineering.stackexchange.com/questions/150616/get-weighted-random-item
	public static ConversationTopic WeightedRandom<T>(this IEnumerable<ConversationTopic> topics)
	{
		int totalWeight = 0; // this stores sum of weights of all elements before current
		ConversationTopic selected = new(TopicName.None); // currently selected element
		Random random = new Random();
		foreach (ConversationTopic topic in topics)
		{
			int weight = topic.Weight; // weight of current element
			int r = random.Next(totalWeight + weight); // random value
			if (r >= totalWeight && weight > 0)
			{ // probability of this is weight/(totalWeight+weight)
				selected = topic;
			} // it is the probability of discarding last selected element and selecting current one instead
			totalWeight += weight; // increase weight sum
		}

		return selected; // when iterations end, selected is some element of sequence. 
	}

	public static string ToRomanNumerals(this int value)
	{
		string romanNumeral;
		switch (value)
		{
			case 1:
				romanNumeral = "I";
				break;
			case 2:
				romanNumeral = "II";
				break;
			case 3:
				romanNumeral = "III";
				break;
			case 4:
				romanNumeral = "IV";
				break;
			case 5:
				romanNumeral = "V";
				break;
			default:
				romanNumeral = "0";
				break;
		}
		return romanNumeral;
	}

	public static string Signed(this int value){
		if(value <= 0){
			return $"{value}";
		}else{
			return $"+{value}";
		}
	}
}
