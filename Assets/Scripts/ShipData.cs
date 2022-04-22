using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData
{
	public string shipName;
	public string captainName;
	public int crewNum;
	public int maxHullPoints;
	public Dictionary<CargoType, int> cargo = new Dictionary<CargoType, int>();
	public int maxWeight;

	public int GetWeight()
	{
		int weight = 0;
		foreach(var pair in cargo)
		{
			weight += (pair.Key.weight * pair.Value);
		}
		return weight;
	}

	public int AddCargo(CargoType type, int amount, bool dontAddIfInsuficcientSpace = false)
	{
		int spaceLeft = maxWeight - GetWeight();
		int weightAdded = type.weight * amount;
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

			int amountToAdd = spaceLeft / type.weight;
			int remainder = amount - amountToAdd;

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
