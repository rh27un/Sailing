using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
	ShipData playerShipData;
	WorldManager manager;

	public Text date;
	public Text shipName;

	public GameObject cargoPanel;
	public GameObject crewPanel;
	public GameObject captainPanel;
	public GameObject messagesPanel;
	public GameObject shipPanel;

	public void Start()
	{
		manager = GetComponent<WorldManager>();
		manager.m_Tick.AddListener(Tick);
	}

	void Tick()
	{
		date.text = manager.GetDate();
	}
	public void Test()
	{
		Debug.Log("Hello");
	}
	public void OpenCargo()
	{
		cargoPanel.SetActive(true);
		crewPanel.SetActive(false);
		captainPanel.SetActive(false);
		messagesPanel.SetActive(false);
		shipPanel.SetActive(false);
	}

	public void OpenCrew()
	{
		cargoPanel.SetActive(false);
		crewPanel.SetActive(true);
		captainPanel.SetActive(false);
		messagesPanel.SetActive(false);
		shipPanel.SetActive(false);
	}

	public void OpenCaptain()
	{
		cargoPanel.SetActive(false);
		crewPanel.SetActive(false);
		captainPanel.SetActive(true);
		messagesPanel.SetActive(false);
		shipPanel.SetActive(false);
	}

	public void OpenMessages()
	{
		cargoPanel.SetActive(false);
		crewPanel.SetActive(false);
		captainPanel.SetActive(false);
		messagesPanel.SetActive(true);
		shipPanel.SetActive(false);
	}


	public void OpenShip()
	{
		cargoPanel.SetActive(false);
		crewPanel.SetActive(false);
		captainPanel.SetActive(false);
		messagesPanel.SetActive(false);
		shipPanel.SetActive(true);
	}

	public void RenameShip(string name)
	{
		playerShipData.shipName = name;
		shipName.text = name;
	}
}
