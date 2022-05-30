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

	public static float costPerDistance = 1f;
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

public class TradeRoute
{
	public Settlement destination;
	public CargoType cargoType;
	public float distance;
	public float profit;

    public override string ToString()
    {
		return $"{cargoType.name} to {destination.name.Replace("(Clone)", string.Empty)} ({distance}km): £{profit}";
	}
}

[CreateAssetMenu(fileName = "Settlement", menuName = "Pirates/Settlement")]
public class Settlement : ScriptableObject
{
	public MapCoords location;
	public MapFaction faction;
	private MapPoint associatedMapPoint;
	private int treasury;
	public Dictionary<string, CargoStores> cargoStores = new Dictionary<string, CargoStores>(); 
	[SerializeField]
	protected List<Feature> features = new List<Feature> ();
	protected List<Settlement> neighbours = new List<Settlement> ();
	protected List<TradeRoute> tradeRoutes = new List<TradeRoute> ();

	public void AddNeighbour(Settlement neighbour)
    {
		if (neighbour != null)
			neighbours.Add (neighbour);
    }
	public void AddNeighbour(IEnumerable<Settlement> neighbour)
    {
		neighbours.AddRange (neighbour);
    }

	public float GetPrice(CargoType type)
    {
		return cargoStores[type.name].price;
    }

	public bool WantsType(CargoType type)
    {
		return cargoStores.ContainsKey (type.name);
    }

	public List<TradeRoute> GetTradeRoutes()
	{
		return tradeRoutes.OrderByDescending(t => t.profit).ToList();
	}

	public int BuyGood(CargoType type, int num)
	{
		if(cargoStores.ContainsKey (type.name))
		{
			if(cargoStores[type.name].quantity >= num)
			{
				cargoStores[type.name].quantity -= num;
				return num;
			}
			else
			{
				int r = num - cargoStores[type.name].quantity;
				cargoStores[type.name].quantity = 0;
				return r;
			}
		}
		return 0;
	}

	public void SellGood(CargoType type, int num)
	{
		if (cargoStores.ContainsKey(type.name))
		{
			cargoStores[type.name].quantity += num;
		}
	}
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
		SetupTrade();
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
	public string Trades()
    {
		string e = string.Empty;
		foreach(var trade in tradeRoutes)
        {
			e += trade.ToString() + "\n";
        }
		return e;
    }

	public void SetupTrade()
    {
		tradeRoutes.Clear();
		foreach(var neighbour in neighbours)
        {
			foreach(var store in cargoStores.Values.Where(s => s.supply > 0))
            {
                if (neighbour.WantsType(store.type))
                {
					var profit = neighbour.GetPrice(store.type) - store.price;
					var distance = MapCoords.DistanceTo(location, neighbour.location);
					profit -= distance * Consts.costPerDistance;

					tradeRoutes.Add(new TradeRoute() { destination = neighbour, cargoType = store.type, distance = distance, profit = profit });
                }
            }
        }
		tradeRoutes = tradeRoutes.OrderByDescending(t => t.profit).ToList();
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
