using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Feature", menuName = "Pirates/Resource Drain")]
public class ResourceDrain : Feature
{
	public CargoType cargoType;
	public int destroyedPerTick;
}

