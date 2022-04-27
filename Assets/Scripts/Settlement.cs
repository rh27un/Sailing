using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResourceTap
{
	public CargoType cargoType;
	public int createdPerTick;
}

public struct ResourceConverter
{
	public CargoType input;
	public CargoType output;
	public float conversionRatio; // + L + no bitches
}

public struct ResourceDrain
{
	public CargoType cargoType;
	public int destroyedPerTick;
}

public struct CargoStores
{
	public CargoType type;
	public int amount;
	public float price;
}

[CreateAssetMenu(fileName = "Settlement", menuName = "Pirates/Settlement")]
public class Settlement : ScriptableObject
{
	public MapCoords location;
	public MapFaction faction;
	private MapPoint associatedMapPoint;
	private int treasury;
	private List<CargoStores> cargoStores; 
}
