using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Consts
{
	public static float demandMultiplier = 2f;
	public static float demandOffset = 121f;
	public static float supplyMultiplier = 10f;
	public static float supplyOffset = 50f;

	public static float CalculatePrice(float quantity, float supply, float demand)
    {
		return 1 / (quantity + supplyOffset + supply * supplyMultiplier ) * (demand + demandOffset) * demandMultiplier;
    }
}
public class Feature : ScriptableObject
{

}

public class CargoStores
{
	public CargoType type;
	public int quantity;
	public float price;
	public float supply;
	public float demand;

    public override string ToString()
    {
		return $"{type.name}: {quantity} - £{price:F2}";
    }
}

[CreateAssetMenu(fileName = "Settlement", menuName = "Pirates/Settlement")]
public class Settlement : ScriptableObject
{
	public MapCoords location;
	public MapFaction faction;
	private MapPoint associatedMapPoint;
	private int treasury;
	[SerializeField]
	public Dictionary<string, CargoStores> cargoStores = new Dictionary<string, CargoStores>(); 
	[SerializeField]
	protected List<Feature> features = new List<Feature> ();

	public void Tick()
	{
		foreach (var feature in features)
		{
			if (feature.GetType() == typeof(ResourceTap))
			{
				var tap = (ResourceTap)feature;
				cargoStores[tap.cargoType.name].quantity += tap.createdPerTick;
			}
			else if (feature.GetType() == typeof(ResourceConverter))
			{
				var converter = (ResourceConverter)feature;
				if (cargoStores[converter.input.name].quantity >= converter.required)
				{
					cargoStores[converter.input.name].quantity -= converter.required;
					cargoStores[converter.output.name].quantity += Mathf.RoundToInt(converter.required * converter.conversionRatio);
                }
			}
			else if (feature.GetType() == typeof(ResourceDrain))
			{
				var drain = (ResourceDrain)feature;
				cargoStores[drain.cargoType.name].quantity = Mathf.Max(0, cargoStores[drain.cargoType.name].quantity - drain.destroyedPerTick);
			}
		}
		foreach(var store in cargoStores.Values)
		{
			store.price = store.type.baseValue * Consts.CalculatePrice(store.quantity, store.supply, store.demand);
		}
	}
	public string Features()
    {
		string e = string.Empty; 
		foreach(var feature in features)
        {
			e += feature.name + ", ";
        }
		return e;
    }
	public string Inventory()
    {
		string e = string.Empty;
		foreach(var store in cargoStores)
        {
			e += store.Value.ToString() + "\n";
        }
		return e;
    }
	public void SetupInventory()
    {
		foreach (var feature in features)
		{
            if (feature.GetType() == typeof(ResourceTap))
            {
				var tap = (ResourceTap)feature;
				if(cargoStores.ContainsKey(tap.cargoType.name))
                {
					var cargo = cargoStores[tap.cargoType.name];
					cargo.supply += tap.createdPerTick;
					cargoStores[tap.cargoType.name] = cargo;
				} 
				else
                {
					var cargo = new CargoStores() { type = tap.cargoType, supply = tap.createdPerTick } ;
					cargoStores.Add(tap.cargoType.name, cargo);
                }
            } 
			else if (feature.GetType() == typeof(ResourceConverter))
            {
				var converter = (ResourceConverter)feature;
				if(cargoStores.ContainsKey(converter.input.name))
                {
					var store = cargoStores[converter.input.name];
					store.demand += converter.required;
					cargoStores[converter.input.name] = store;
                }
				else
				{
					var cargo = new CargoStores() { type = converter.input, demand = converter.required };
					cargoStores.Add(converter.input.name, cargo);

				}
				if(cargoStores.ContainsKey(converter.output.name))
                {
					var store = cargoStores[converter.output.name];
					store.supply += converter.required * converter.conversionRatio;
					cargoStores[converter.output.name] = store;
                } 
				else
                {
					var cargo = new CargoStores() { type = converter.output, supply = converter.required * converter.conversionRatio };
					cargoStores.Add(converter.output.name, cargo);
				}
            }
			else if(feature.GetType() == typeof(ResourceDrain))
            {
				var drain = (ResourceDrain)feature;
				if (cargoStores.ContainsKey(drain.cargoType.name))
				{
					var store = cargoStores[drain.cargoType.name];
					store.demand += drain.destroyedPerTick;
					cargoStores[drain.cargoType.name] = store;
				}
				else
				{
					var cargo = new CargoStores() { type = drain.cargoType, demand = drain.destroyedPerTick };
					cargoStores.Add(drain.cargoType.name, cargo);
				}
			}
			else
            {
				throw new System.Exception();
            }
		}
    }
}
