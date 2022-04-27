using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData
{
	public string shipName;
	public string captainName;
	public int crewNum;
	public int maxHullPoints;
	public Dictionary<CargoType, float> cargo = new Dictionary<CargoType, float>();
	public int maxWeight;

	public float GetWeight()
	{
		float weight = 0;
		foreach(var pair in cargo)
		{
			weight += (pair.Key.weight * pair.Value);
		}
		return weight;
	}

	public float AddCargo(CargoType type, int amount, bool dontAddIfInsuficcientSpace = false)
	{
		float spaceLeft = maxWeight - GetWeight();
		float weightAdded = type.weight * amount;
		if (spaceLeft > weightAdded)
		{
			if (cargo.ContainsKey(type))
			{
				cargo[type] += amount;
			}
			else
			{
				cargo.Add(type, amount);
			}
			return 0;
		}
		else
		{
			if (dontAddIfInsuficcientSpace)
				return amount;

			float amountToAdd = spaceLeft / type.weight;
			float remainder = amount - amountToAdd;

			if (cargo.ContainsKey(type))
			{
				cargo[type] += amountToAdd;
			}
			else
			{
				cargo.Add(type, amountToAdd);
			}

			return remainder;
		}
	}
}
