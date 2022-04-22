using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Ship
{ 
    protected override void Steer()
	{
        rudder = Input.GetAxis("Horizontal");
        sailPercent = Mathf.Clamp(sailPercent + Input.GetAxis("Vertical") * Time.deltaTime, 0f, 1f);
		if (Input.GetButtonDown("Anchor"))
		{
			isAnchored = !isAnchored;
		}
	}


}
