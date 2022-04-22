using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LiveMapPoint : MapPoint // live map data for nearby ships updated in realtime from its transform i.e player ship
{
	public Transform tracking;
	public override void UpdatePoint(float deltaTime)
	{
		coords = MapCoords.FromTransform(tracking);
	}

	//public SimulatedMapPoint ConvertToSimulated()
	//{
	//	return new SimulatedMapPoint
	//	{
	//		coords = coords,
	//		visible = visible,
	//		type = type,
	//		faction = faction,
	//		associatedObject = associatedObject
	//	};
	//}
}