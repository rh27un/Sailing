using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OrderState
{
	None = 0,                   
	Created = 1,                
	WaitingOnSupply = 2,        
	InTransit = 3,              
	Completed = 4,              
	Interrupted = 5,            
	Failed = 6
}
public struct Order
{
	public CargoType type;
	public OrderState state;
	public Settlement origin;
	public Settlement destination;
}

public class WorldManager : MonoBehaviour
{
	public UnityEvent m_Tick = new UnityEvent();

	public int year;
	public int month;
	public int day;
	public System.DateTime date;
	public float tickSpeed;
	public List<CargoType> cargoTypes = new List<CargoType>();
	protected List<SimulatedShip> ships;
	[SerializeField]
	protected List<Settlement> settlements = new List<Settlement>();
	[SerializeField]
	protected List<Settlement> liveSettlements = new List<Settlement>();
	private IEnumerator coroutine;
	[SerializeField]
	protected MapData map;

	public string GetDate()
	{
		string format = "MMMM " + Day(date.Day) + " yyy";
		return date.ToString(format);
	}
	private string Day(int day)
	{
		switch (day)
		{
			case 1:
			case 21:
			case 31:
				return "d\"st\"";
			case 2:
			case 22:
				return "d\"nd\"";
			case 3:
			case 23:
				return "d\"rd\"";
			default:
				return "d\"th\"";
		}
	}
	private void Start()
	{
		date = new System.DateTime(year, month, day);

		coroutine = DoTick(tickSpeed);

		StartCoroutine(coroutine);
		map.AddSettlements(settlements);

		foreach(var settlement in settlements)
        {
			var live = Instantiate(settlement);
			live.SetupInventory();
			liveSettlements.Add(live);
        }
	}

	private void Tick()
	{
		date = date.AddDays(1);
		m_Tick.Invoke();
	}

	private IEnumerator DoTick(float tickspeed)
	{
		while (true)
		{
			yield return new WaitForSeconds(tickspeed);
			Tick();
		}
	}
}

// Simulated AI ships, all ships on the map and in-game follow this logic except in battle
public class SimulatedShip
{
	protected MapCoords coords;
	protected ShipData data;
}