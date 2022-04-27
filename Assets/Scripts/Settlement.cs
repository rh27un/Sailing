using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Feature : ScriptableObject
{

}

public struct CargoStores
{
	public CargoType type;
	public int quantity;
	public float price;
	public float supply;
	public float demand;
}

[CreateAssetMenu(fileName = "Settlement", menuName = "Pirates/Settlement")]
public class Settlement : ScriptableObject
{
	public MapCoords location;
	public MapFaction faction;
	private MapPoint associatedMapPoint;
	private int treasury;
	[SerializeField]
	public List<CargoStores> cargoStores = new List<CargoStores>(); 
	[SerializeField]
	protected List<Feature> features = new List<Feature> ();

	public void SetupInventory()
    {
		foreach (var feature in features)
		{
            if (feature.GetType() == typeof(ResourceTap))
            {
				var tap = (ResourceTap)feature;
				if(cargoStores.Any(cs => cs.type == tap.cargoType))
                {
					var store = cargoStores.Single(cs => cs.type == tap.cargoType);
					store.supply += tap.createdPerTick;
				} 
				else
                {
					var cargo = new CargoStores() {  type = tap.cargoType, supply = tap.createdPerTick };
					cargoStores.Add(cargo);
                }
            } 
			else if (feature.GetType() == typeof(ResourceConverter))
            {
				var converter = (ResourceConverter)feature;
				if(cargoStores.Any(cs => cs.type == converter.input))
                {
					var store = cargoStores.Single(cs => cs.type == converter.input);
					store.demand += 1;
                }
				else
				{
					var cargo = new CargoStores() { type = converter.input, demand = 1 };
					cargoStores.Add(cargo);

				}
				if(cargoStores.Any(cs => cs.type == converter.output))
                {
					var store = cargoStores.Single(cs => cs.type == converter.output);
					store.supply += converter.conversionRatio;
                } 
				else
                {
					var cargo = new CargoStores() { type = converter.output, supply = converter.conversionRatio };
					cargoStores.Add(cargo);
				}
            }
			else if(feature.GetType() == typeof(ResourceDrain))
            {
				var drain = (ResourceDrain)feature;
				if (cargoStores.Any(cs => cs.type == drain.cargoType))
				{
					var store = cargoStores.Single(cs => cs.type == drain.cargoType);
					store.demand += drain.destroyedPerTick;
				}
				else
				{
					var cargo = new CargoStores() { type = drain.cargoType, demand = drain.destroyedPerTick };
					cargoStores.Add(cargo);
				}
			}
			else
            {
				throw new System.Exception();
            }
		}
    }
}
