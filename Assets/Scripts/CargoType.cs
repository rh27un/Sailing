using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cargo Type", menuName = "Pirates/Cargo Type", order = 2)]
public class CargoType : ScriptableObject
{
	public int baseValue;
	public float weight;
}
