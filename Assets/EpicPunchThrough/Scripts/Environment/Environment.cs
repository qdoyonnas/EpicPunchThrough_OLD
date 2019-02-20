using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

[Serializable]
public class Environment
{
    [Serializable]
    public struct BackgroundField
    {
        public int depth;
        public Sprite[] sprites;
    }

    public string name;
    public string sceneName;
    public BackgroundField[] backgrounds;
    public GameObject[] obstacles;
    public WorldBounds agentBounds;
}
