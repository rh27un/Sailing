using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TradeShip
{
	public MapCoords location;
	public ShipData shipData;
	protected Settlement curSettlement;
	protected TradeRoute route;
	protected Vector2 start;
	protected Vector2 end;
	protected float t = 0f;

	public void GenerateData(string shipName)
	{
		shipData = new ShipData()
		{
			shipName = shipName,
			captainName = "George",
			crewNum = 12,
			maxHullPoints = 100,
			maxWeight = 1000,
			cargo = new Dictionary<CargoType, float>()
		};
	}

	public void SetSettlement(Settlement settlement)
	{
		curSettlement = settlement;
		location = settlement.location;
	}

	public void Tick()
	{
		if(route != null)
		{
			var loc = Vector2.Lerp(start, end, t);
			location = new MapCoords() { x = Mathf.RoundToInt(loc.x), y = Mathf.RoundToInt(loc.y)};
			location.rotation = MapCoords.AngleBetween(location, route.destination.location);
			if (t >= 1f)
			{
				curSettlement = route.destination;
				Debug.Log("Selling " + route.cargoType + " to " + curSettlement.name);
				foreach (var cargo in shipData.cargo)
				{
					curSettlement.SellGood(cargo.Key, (int)cargo.Value);
				}
				route = null;
			}
			else
			{
				t += 1 / (start - end).magnitude;
			}
		}
		else if(curSettlement != null)
		{
			var routes = curSettlement.GetTradeRoutes();
			if (routes.Count > 0)
			{
				var bestRoute = routes[0];
				start = curSettlement.location.ToVector2();
				end = bestRoute.destination.location.ToVector2();
				t = 0;
				var otherRoutes = routes.Where(r => r.destination == bestRoute.destination && r.profit > r.distance).ToList();
				int weightLeft = shipData.maxWeight;
				if (otherRoutes.Count > 0)
				{
					weightLeft /= 2;
				}
				shipData.AddCargo(bestRoute.cargoType, curSettlement.BuyGood(bestRoute.cargoType, Mathf.FloorToInt(weightLeft / bestRoute.cargoType.weight)));
				for (int i = 0; i < otherRoutes.Count; i++)
				{
					if (i < otherRoutes.Count - 1 || Mathf.FloorToInt(weightLeft / 2) > 0)
					{
						weightLeft = Mathf.FloorToInt(weightLeft / 2);
					}
					else
					{
						shipData.AddCargo(otherRoutes[i].cargoType, curSettlement.BuyGood(bestRoute.cargoType, Mathf.FloorToInt(weightLeft / bestRoute.cargoType.weight)));
						break;
					}
					shipData.AddCargo(otherRoutes[i].cargoType, curSettlement.BuyGood(bestRoute.cargoType, Mathf.FloorToInt(weightLeft / bestRoute.cargoType.weight)));
				}
				route = bestRoute;
				Debug.Log("Buying " + bestRoute.cargoType + " from " + curSettlement.name);
				curSettlement = null;
			}
		}
	}
}
