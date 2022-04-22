using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum MapType
{
	Misc = 0,
	Settlement = 1,
	Player = 2,
	Ship = 3
}
public enum MapFaction
{
	None = 0,
	Player = 1,
	Pirate = 2,
	Elvonie = 3,
	Rockstone = 4,
	Henland = 5
}

[System.Serializable]
public struct MapCoords
{
	public MapCoords(int _x, int _y, float _rot)
	{
		x = _x;
		y = _y;
		rotation = _rot;
	}
	public int x;
	public int y;
	public float rotation;
	public static Vector2 ToVector2(MapCoords mapCoords)
	{
		return new Vector2(mapCoords.x, mapCoords.y);
	}
	public static MapCoords FromWorldVector3(Vector3 world)
	{
		return new MapCoords((int)(world.x), (int)(world.z), 0f);
	}

	public static MapCoords FromTransform(Transform transform)
	{
		return new MapCoords((int)(transform.position.x), (int)(transform.position.z), transform.rotation.eulerAngles.y);
	}
}


public class MapData : MonoBehaviour
{
	public List<Sprite> sprites = new List<Sprite>();
	public List<Color> colours = new List<Color>();

	[SerializeField]
	protected List<MapPoint> stationaryPoints = new List<MapPoint>();

	protected List<LiveMapPoint> livePoints = new List<LiveMapPoint>();

	protected List<MapPoint> simulatedPoints = new List<MapPoint>();

	protected Image bgImage;
	protected RectTransform bgRect;
	protected Dictionary<MapPoint, Image> images = new Dictionary<MapPoint, Image>();
	private bool fadingIn;
	private bool fadingOut;
	private float fadeStart;
	[SerializeField]
	protected float fadeSpeed;

	[SerializeField]
	protected float zoom = 1f;
	[SerializeField]
	protected float zoomSensitivity;
	[SerializeField]
	protected float zoomMin;
	[SerializeField]
	protected float zoomMax;

	protected Vector3 panStart;
	[SerializeField]
	protected float panSensitivity;
	[SerializeField]
	protected Vector2 mapOffset;

	[SerializeField]
	protected float imageScale;
	[SerializeField]
	protected CameraRotation camRot;
	public float bgZoomSpeed;
	public float bgPanSpeed;

	protected Transform shipList;
	public Image DrawPoint(MapPoint mapPoint)
	{
		GameObject go = Instantiate(new GameObject(mapPoint.name), transform);
		Image image = go.AddComponent<Image>();
		image.sprite = sprites[(int)mapPoint.type];
		image.color = Color.clear;
		RectTransform rect = go.GetComponent<RectTransform>();
		//rect.SetParent(transform);
		rect.anchoredPosition = MapCoords.ToVector2(mapPoint.coords) * zoom + mapOffset;
		rect.localScale *= imageScale;
		rect.rotation = Quaternion.Euler(0f, 0f, mapPoint.coords.rotation);
		return image;
	}


	public void Start()
	{
		bgImage = GetComponent<Image>();
		// Create map point for the player.
		Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		var playerPoint = (LiveMapPoint)ScriptableObject.CreateInstance("LiveMapPoint");
		playerPoint.name = "Player";
		playerPoint.coords = MapCoords.FromTransform(playerTransform);
		playerPoint.visible = true;
		playerPoint.type = MapType.Player;
		playerPoint.faction = MapFaction.Player;
		playerPoint.associatedObject = playerTransform.gameObject;
		playerPoint.tracking = playerTransform;
		bgRect = gameObject.GetComponent<RectTransform>();
		livePoints.Add(playerPoint);
		// Draw player point
		images.Add(livePoints[0], DrawPoint(livePoints[0]));
		// draw stationary points
		foreach (MapPoint mapPoint in stationaryPoints)
		{
			images.Add(mapPoint, DrawPoint(mapPoint));
		}
	}

	public void Update()
	{


		// Update Live Points
		foreach (var point in livePoints)
		{
			point.coords = MapCoords.FromTransform(point.tracking);
			var rect = images[point].GetComponent<RectTransform>();
			rect.anchoredPosition = MapCoords.ToVector2(point.coords);
			rect.rotation = Quaternion.Euler(0f, 0f, -point.coords.rotation);
		}

		// TODO: update simulated points

		if (!fadingIn && !fadingOut)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				panStart = Input.mousePosition - (Vector3)mapOffset;
			}
			if (Input.GetButton("Fire1"))
			{

				mapOffset = Input.mousePosition - panStart;
			}
			zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity, zoomMin, zoomMax);
			bgRect.anchoredPosition = mapOffset;
			bgRect.localScale = Vector3.one * zoom;
			foreach (var image in images)
			{
				var rect = image.Value.GetComponent<RectTransform>();
				rect.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z) * imageScale;
			}
			if (zoom == zoomMax)
			{
				FadeOut();

			}
		}

		if (fadingIn)
		{
			float t = (Time.time - fadeStart) * fadeSpeed;

			bgImage.color = Color.Lerp(Color.clear, Color.white, t);
			Color[] factionColors = new Color[colours.Count];
			for (int i = 0; i < colours.Count; i++)
			{
				factionColors[i] = Color.Lerp(Color.clear, colours[i], t);
			}

			foreach (var image in images)
			{
				image.Value.color = factionColors[(int)image.Key.faction];
			}
			if (t >= 1f)
			{
				fadingIn = false;
			}
		}
		else if (fadingOut)
		{
			float t = (Time.time - fadeStart) * fadeSpeed;

			bgImage.color = Color.Lerp(Color.white, Color.clear, t);
			Color[] factionColors = new Color[colours.Count];
			for (int i = 0; i < colours.Count; i++)
			{
				factionColors[i] = Color.Lerp(colours[i], Color.clear, t);
			}

			foreach (var image in images)
			{
				image.Value.color = factionColors[(int)image.Key.faction];
			}

			if (t >= 1f)
			{
				fadingOut = false;
				camRot.gameObject.SetActive(true);
				camRot.SetYAxis(0.98f);
				gameObject.SetActive(false);

			}
		}
	}

	public void FadeIn()
	{
		if (!fadingIn && !fadingOut)
		{
			fadeStart = Time.time;
			fadingIn = true;
			zoom = zoomMax - zoomMax * 0.01f;
		}
	}
	public void FadeOut()
	{
		if (!fadingOut && !fadingIn)
		{
			fadeStart = Time.time;
			fadingOut = true;
		}
	}
}





//public class SimulatedMapPoint : MapPoint
//{
//	// simulated nonplayer ships that follow a path

//	//public LiveMapPoint ConvertToLive()
//	//{
//	//	return new LiveMapPoint
//	//	{
//	//		coords = coords,
//	//		visible = visible,
//	//		type = type,
//	//		faction = faction,
//	//		associatedObject = associatedObject
//	//	};
//	//}
//}
