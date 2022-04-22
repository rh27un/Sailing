using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
	protected WindController wind;
	[SerializeField]
	protected float agility; // how steering the boat does
	[SerializeField]
	protected float swiftness; // how fast the boat does

	protected bool isAnchored;

	[SerializeField]
	protected float drag;

	protected float rudder; // how steering the boat is doing, -1.0 to 1.0
	protected float sailPercent; // how sailing the boat is doing, 0.0 to 1.0

	protected float direction; // what direction the boat goes
	protected float speed; // what speed the boat goes

	const float sailMoveScale = 0.5f;

	new protected Rigidbody rigidbody;

	protected Dictionary<GameObject, float> sails = new Dictionary<GameObject, float>();
	protected GameObject[] masts;
	// Start is called before the first frame update
	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		wind = GameObject.FindGameObjectWithTag("GameController").GetComponent<WindController>();
		// i hope this was a mistake?
		//wind = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
		GameObject[] sailsArray = GameObject.FindGameObjectsWithTag("Sail");
		masts = GameObject.FindGameObjectsWithTag("Mast");
		foreach (GameObject sail in sailsArray)
		{
			sails.Add(sail, sail.transform.localScale.y);
		}
	}

	// Update is called once per frame
	void Update()
	{
		Steer();

		//direction += ((rudder * agility) + ((wind.windDirection * wind.windStrength) - direction) * 0.001f) * speed;
		//transform.rotation = Quaternion.Euler(0f, direction, 0f);
		rigidbody.AddTorque(0f, ((rudder * agility) * Time.deltaTime) * speed, 0f);

		float dot = Vector3.Dot(transform.forward, wind.transform.forward);
		speed = wind.windStrength * sailPercent * (dot + 0.5f) * swiftness;
		rigidbody.AddForce(transform.forward * speed * Time.deltaTime);
		rigidbody.drag = isAnchored ? 2f : drag - Vector3.Dot(transform.forward, rigidbody.velocity.normalized);
		//transform.position += transform.forward * speed * Time.deltaTime;

		foreach(GameObject sail in sails.Keys)
		{
			float prevScale = sail.transform.localScale.y;
			float newScale = sails[sail] * sailPercent;
			float diff = prevScale - newScale;
			sail.transform.localScale = new Vector3(sail.transform.localScale.x, newScale, sail.transform.localScale.z);
			sail.transform.localPosition += new Vector3(0f, diff) * sailMoveScale;

		}
		if (dot > 0.5f)
		{
			foreach(GameObject mast in masts)
			{
				mast.transform.rotation = wind.transform.rotation;
			}
		}
	}

	protected virtual void Steer()
	{

	}
}
