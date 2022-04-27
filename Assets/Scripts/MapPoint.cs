using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint
{
	public string name;
	public MapCoords coords;
	public bool visible;
	public MapType type;
	public MapFaction faction;
	public GameObject associatedObject;
	public virtual void UpdatePoint(float deltaTime)
	{

	}
}

