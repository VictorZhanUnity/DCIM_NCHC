using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class HeatmapManager : MonoBehaviour
{
    [Header("Heatmap Plane")]
    public Transform heatmapPlane;  // 綁你的地板
    public Vector2 planeSize = new Vector2(10, 20);  // 地板世界尺寸

    [Header("Temperature Range")]
    public float minTemp = 18f;
    public float maxTemp = 30f;

    [Header("Heatmap Settings")]
    public int textureSize = 512;

    [Header("References")]
    public Material blurMaterial;
    public Material colorMaterial;
    public Renderer targetRenderer;   // 地板 Mesh Renderer

    public Texture2D rawTex;
    public RenderTexture blurRT;
    public RenderTexture finalRT;

    void Start()
    {
        InitTextures();
    }

    [Button]
    void InitTextures()
    {
        rawTex = new Texture2D(textureSize, textureSize, TextureFormat.RFloat, false);

        //blurRT = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat);
        //finalRT = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);

        // 結果貼到地板
        targetRenderer.material.SetTexture("_MainTex", finalRT);
    }

    // ----------------------------------------------------------
    // Public API：你只要呼叫 UpdateHeatmap(sensorList)
    // ----------------------------------------------------------
    public void UpdateHeatmap(List<SensorData> sensors)
    {
        ClearRawTexture();

        foreach (var s in sensors)
        {
            Vector2 uv = WorldToUV(s.worldPos);
            float n = NormalizeTemp(s.temperature);

            if (uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1)
                AddSensorToRaw(uv, n);
        }

        rawTex.Apply();

        // Step 2: Blur
        Graphics.Blit(rawTex, blurRT, blurMaterial);

        // Step 3: Color
        Graphics.Blit(blurRT, finalRT, colorMaterial);
    }

    // ----------------------------------------------------------
    // 工具：世界座標 -> UV(0~1)
    // ----------------------------------------------------------
    Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector3 local = heatmapPlane.InverseTransformPoint(worldPos);

        float u = Mathf.InverseLerp(-planeSize.x / 2, planeSize.x / 2, local.x);
        float v = Mathf.InverseLerp(-planeSize.y / 2, planeSize.y / 2, local.z);

        return new Vector2(u, v);
    }

    // ----------------------------------------------------------
    // 工具：溫度 Normalize → 0~1
    // ----------------------------------------------------------
    float NormalizeTemp(float t)
    {
        return Mathf.InverseLerp(minTemp, maxTemp, t);
    }

    // ----------------------------------------------------------
    // 工具：繪 RawTexture（黑白）
    // ----------------------------------------------------------
    void AddSensorToRaw(Vector2 uv, float n)
    {
        int x = Mathf.RoundToInt(uv.x * (textureSize - 1));
        int y = Mathf.RoundToInt(uv.y * (textureSize - 1));

        rawTex.SetPixel(x, y, new Color(n, n, n));
    }

    void ClearRawTexture()
    {
        for (int y = 0; y < textureSize; y++)
            for (int x = 0; x < textureSize; x++)
                rawTex.SetPixel(x, y, Color.black);
    }

    void OnDestroy()
    {
        if (blurRT) blurRT.Release();
        if (finalRT) finalRT.Release();
    }


    public List<Transform> rackList;
    
    [Button]
    private void Test()
    {
        List<SensorData> sensors = new List<SensorData>();

        foreach (var rack in rackList)
        {
            Vector3 pos = rack.transform.position;
            float temp = Random.Range(minTemp, maxTemp);

            sensors.Add(new SensorData(pos, temp));
        }

        UpdateHeatmap(sensors);
    }
}
