using NaughtyAttributes;
using UnityEngine;

public class HeatmapTest : MonoBehaviour
{
    [SerializeField] Material heatmapMaterial;

    public Vector4[] points = new Vector4[32];

    public int amount = 100;
    
    /*
    int count = sensors.Count;

        for (int i = 0; i < count; i++)
    {
        points[i] = new Vector4(
            sensors[i].worldPos.x, 
            sensors[i].worldPos.z,
            sensors[i].temperature01, 
            0
        );
    }*/
    

    [Button]
    private void Test()
    {
        heatmapMaterial.SetInt("_PointCount", amount);
        heatmapMaterial.SetVectorArray("_Points", points);
    }
}
