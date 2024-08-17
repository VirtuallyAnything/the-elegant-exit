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

	public static Godot.Collections.Array<EnemyInterest> ToConversationTopics(this Godot.Collections.Array<TopicName> topicNames)
	{
		Godot.Collections.Array<EnemyInterest> conversationTopics = new();
		foreach (TopicName topic in topicNames)
		{
			EnemyInterest conversationTopic = new(topic);
			conversationTopics.Add(conversationTopic);
		}
		return conversationTopics;
	}

	public static EnemyInterest WeightedRandom<T>(this Godot.Collections.Array<EnemyInterest> topics)
	{
		int totalWeight = 0; // this stores sum of weights of all elements before current
		EnemyInterest selected = new(TopicName.None); // currently selected element
		Random random = new Random();
		foreach (EnemyInterest topic in topics)
		{
			int weight = topic.Weight; // weight of current element
			int r = random.Next(totalWeight + weight); // random value
			if (r >= totalWeight)
			{ // probability of this is weight/(totalWeight+weight)
				selected = topic;
			} // it is the probability of discarding last selected element and selecting current one instead
			totalWeight += weight; // increase weight sum
		}

		return selected; // when iterations end, selected is some element of sequence. 
	}
}
