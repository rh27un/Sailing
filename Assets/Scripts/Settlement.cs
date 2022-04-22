using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct CargoStores
{
	public CargoType type;
	public int amount;
	public bool isProducing;
	public bool isConsuming;
}
public class Settlement
{
	const int amountConsumed = 100;
	const int amountProduced = 100;

	public string name;
	public MapCoords location;
	public MapFaction faction;
	private MapPoint associatedMapPoint;
	private int treasury;
	private List<CargoStores> cargoStores; 
}
