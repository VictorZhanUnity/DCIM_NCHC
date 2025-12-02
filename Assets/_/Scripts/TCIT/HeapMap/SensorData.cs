
using UnityEngine;

public class SensorData
{
    public Vector3 worldPos;
    public float temperature;

    public SensorData(Vector3 pos, float temp)
    {
        worldPos = pos;
        temperature = temp;
    }
}