using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
public class CameraRotation : MonoBehaviour
{
	protected CinemachineFreeLook freeLookComponent;
	[SerializeField]
	protected MapData mapData;


	public static bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		//Debug.Log(results[0].gameObject.name);
		return results.Count > 0;
	}

	// Start is called before the first frame update
	void Start()
	{
		freeLookComponent = GetComponent<CinemachineFreeLook>();
	}

	// Update is called once per frame
	void Update()
	{
		bool isPointerEtc = IsPointerOverUIObject();
		if (Input.GetButtonDown("Fire1") && !isPointerEtc)
		{
			Cursor.lockState = CursorLockMode.Locked;

			freeLookComponent.m_XAxis.m_MaxSpeed = 800;
		}
		else if (Input.GetButtonUp("Fire1"))
		{
			Cursor.lockState = CursorLockMode.None;
			freeLookComponent.m_XAxis.m_MaxSpeed = 0;
		}

		if (freeLookComponent.m_YAxis.Value == 1f)
		{

			mapData.gameObject.SetActive(true);
			mapData.FadeIn();
			gameObject.SetActive(false);
		}
	}

	public void EnableRotation()
	{
		freeLookComponent.enabled = true;
	}

	public void SetYAxis(float value)
	{
		freeLookComponent.m_YAxis.Value = value;
	}
}
