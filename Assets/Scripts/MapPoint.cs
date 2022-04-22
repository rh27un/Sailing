using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Point", menuName = "Pirates/MapPoint", order = 1)]
public class MapPoint : ScriptableObject
{
	public string pointName;
	public MapCoords coords;
	public bool visible;
	public MapType type;
	public MapFaction faction;
	public GameObject associatedObject;
	public virtual void UpdatePoint(float deltaTime)
	{

	}
}

