using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameOptions
{
	public float gravity;
	public float maximumSpeed;
	public float vF2NRatio;
	public WorldBounds worldBounds;
}

public struct WorldBounds
{
	public bool leftBound;
	public bool rightBound;
	public bool topBound;
	public bool bottombound;
	public float minX;
	public float maxX;
	public float width
	{
		get { return maxX - minX; }
	}
	public float minY;
	public float maxY;
	public float height
	{
		get { return maxY - minY; }
	}
}