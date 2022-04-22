using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVane : MonoBehaviour
{
	protected Transform shipTransform;
    protected Transform windDirection;
    protected WindController windController;
	protected Vector3 offset;

	private void Start()
	{
		shipTransform = GameObject.FindGameObjectWithTag("Player").transform;
		windDirection = GameObject.FindGameObjectWithTag("GameController").transform;
		if(windDirection == null || shipTransform == null)
		{
			Destroy(gameObject);
		}
		windController = windDirection.gameObject.GetComponent<WindController>();

		offset = transform.position - shipTransform.position;
	}
	void Update()
    {
		transform.rotation = windDirection.rotation;
		transform.localScale = new Vector3(2f, 2f, windController.windStrength * 20f);
		transform.position = shipTransform.position + offset;
    }
}
