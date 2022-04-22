using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    protected RawImage compass;

    [SerializeField]
    protected Transform player;
    void Start()
    {
        compass = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        compass.uvRect = new Rect(player.localEulerAngles.y / 360f, 0, 1, 1);
    }
}
