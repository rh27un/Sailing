using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Feature", menuName = "Pirates/Resource Converter")]
public class ResourceConverter : Feature
{
	public CargoType input;
	public CargoType output;
	public float conversionRatio; // + L + no bitches
}
