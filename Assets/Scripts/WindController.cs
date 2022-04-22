using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{

    [SerializeField]
    public float windDirection; // degrees around the y axis

    [SerializeField]
    public float windStrength;

    [SerializeField]
    protected float directionNoiseScale; // scale at which the wind direction changes

    [SerializeField]
    protected float strengthNoiseScale; // scale at which the wind strength changes

    [SerializeField]
    protected float prevailingWind; // overall wind direction

    [SerializeField]
    protected float windChange; // range of degrees wind direction can change, centred on the prevailing wind

    protected float directionX;
    protected float strengthX;
    protected float mathsPrev;
    // Start is called before the first frame update
    void Start()
    {
        directionX = Random.value * 10;
        strengthX = Random.value * 10;

        mathsPrev = (prevailingWind - windChange/2) % 360;
    }

    // Update is called once per frame
    void Update()
    {
        windDirection = mathsPrev + (Mathf.PerlinNoise(directionX, Time.time * directionNoiseScale) * windChange);
        windStrength = Mathf.PerlinNoise(strengthX, Time.time * strengthNoiseScale);
        transform.rotation = Quaternion.Euler(0f, windDirection, 0f);
    }
}
