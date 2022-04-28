using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public override string ToString()
    {
		return $"{x}, {y}";
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
	protected Dictionary<string, MapPoint> mapPoints = new Dictionary<string, MapPoint>();
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
	protected Vector2 zoomOffset;

	[SerializeField]
	protected float imageScale;
	[SerializeField]
	protected CameraRotation camRot;
	public float bgZoomSpeed;
	public float bgPanSpeed;

	[SerializeField]
	protected GameObject infoPanel;
	protected WorldManager worldManager;

	protected Transform shipList;
	public Image DrawPoint(MapPoint mapPoint)
	{
		GameObject go = Instantiate(new GameObject(mapPoint.name), transform);
		go.name = mapPoint.name;
		Image image = go.AddComponent<Image>();
		image.sprite = sprites[(int)mapPoint.type];
		image.color = Color.clear;
		RectTransform rect = go.GetComponent<RectTransform>();
		//rect.SetParent(transform);
		rect.anchoredPosition = MapCoords.ToVector2(mapPoint.coords) * zoom + mapOffset;
		rect.localScale *= imageScale;
		rect.rotation = Quaternion.Euler(0f, 0f, mapPoint.coords.rotation);
		Button btn = go.AddComponent<Button>();
		
		return image;
	}


	public void Start()
	{
		worldManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldManager>();
		bgImage = GetComponent<Image>();
		// Create map point for the player.
		Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		var playerPoint = new LiveMapPoint()
		{
			name = "Player",
			coords = MapCoords.FromTransform(playerTransform),
			visible = true,
			type = MapType.Player,
			faction = MapFaction.Player,
			associatedObject = playerTransform.gameObject,
			tracking = playerTransform
		};
		bgRect = gameObject.GetComponent<RectTransform>();
		livePoints.Add(playerPoint);
		// Draw player point
		images.Add(livePoints[0], DrawPoint(livePoints[0]));
		mapPoints.Add("Player", playerPoint);
		// draw stationary points
		foreach (MapPoint mapPoint in stationaryPoints)
		{
			images.Add(mapPoint, DrawPoint(mapPoint));
			mapPoints.Add(mapPoint.name, mapPoint);
		}
	}

	public void AddSettlements(List<Settlement> settlements)
    {
		foreach (Settlement settlement in settlements)
        {
			var mapPoint = new MapPoint()
			{
				name = settlement.name,
				coords = settlement.location,
				type = MapType.Settlement,
				faction = settlement.faction
			};

			images.Add(mapPoint, DrawPoint(mapPoint));
			mapPoints.Add(mapPoint.name, mapPoint);
        }
    }

	public void Update()
	{

		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		if(results.Count > 0 && results[0].gameObject != gameObject)
        {
			if (mapPoints.ContainsKey(results[0].gameObject.name))
			{
				infoPanel.SetActive(true);
				infoPanel.transform.position = Input.mousePosition + Vector3.up * 420f;
				var selectedPoint = mapPoints[results[0].gameObject.name];
				var texts = infoPanel.GetComponentsInChildren<Text>();
				texts[0].text = selectedPoint.name;
				texts[1].text = selectedPoint.type.ToString();
				texts[2].text = selectedPoint.faction.ToString();
				texts[3].text = worldManager.GetFeatures(selectedPoint.name);
				texts[4].text = worldManager.GetInventory(selectedPoint.name);
			}
        } else
        {
			infoPanel.SetActive(false);
        }
		// Update Live Points
		foreach (var point in livePoints)
		{
			point.coords = MapCoords.FromTransform(point.tracking);
			var rect = images[point].GetComponent<RectTransform>();
			rect.anchoredPosition = MapCoords.ToVector2(point.coords);
			rect.rotation = Quaternion.Euler(0f, 0f, -point.coords.rotation);
		}

		// TODO: update simulated points

		if (!fadingOut)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				panStart = Input.mousePosition - (Vector3)mapOffset;
			}
			if (Input.GetButton("Fire1"))
			{

				mapOffset = Input.mousePosition - panStart;
			}
			zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity * zoom, zoomMin, zoomMax);
			//man am i bad at maths
			var someConst = zoom;
			var mousePos = (Vector2)bgRect.InverseTransformPoint(Input.mousePosition);
			bgRect.localScale = Vector3.one * zoom;
			var newMousePos = (Vector2)bgRect.InverseTransformPoint(Input.mousePosition);

			var mouseDelta = mousePos - newMousePos;
			zoomOffset += mouseDelta * someConst;
			bgRect.anchoredPosition = mapOffset - zoomOffset;
			foreach (var image in images)
			{
				var rect = image.Value.GetComponent<RectTransform>();
				rect.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z) * imageScale;
			}
			infoPanel.GetComponent<RectTransform>().localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
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
				camRot.SetYAxis(0.94f);
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
