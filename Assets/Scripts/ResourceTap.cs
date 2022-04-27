using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Pirates/Resource Tap")]
public class ResourceTap : Feature
{
	public CargoType cargoType;
	public int createdPerTick;
}