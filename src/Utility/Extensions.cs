using Godot;
using System;

public static class Extensions : Object
{
	public static Vector2[] ToVertices(this Vector2 size, Vector2 origin, bool centerVerticesAroundOrigin = false)
	{
		Vector2[] vertices = new Vector2[4];
		if (centerVerticesAroundOrigin)
		{
			vertices[0] = new Vector2(origin.X + size.X/2, origin.Y - size.Y/2);
			vertices[1] = new Vector2(origin.X + size.X/2, origin.Y + size.Y/2);
			vertices[2] = new Vector2(origin.X - size.X/2, origin.Y + size.Y/2);
			vertices[3] = new Vector2(origin.X - size.X/2, origin.Y - size.Y/2);
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
}
