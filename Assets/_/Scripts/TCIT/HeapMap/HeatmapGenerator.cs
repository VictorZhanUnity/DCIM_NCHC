using NaughtyAttributes;
using UnityEngine;

public class HeatmapGenerator : MonoBehaviour
{
    public int textureSize = 512;
    public Texture2D rawTex;

    public RenderTexture blurTemp;
    public RenderTexture finalHeatmap;
    public Material blurMat;
    public Material colorMat;

    void InitRT()
    {
        blurTemp = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat);
        finalHeatmap = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
    }

    [Button]
    public void GenerateHeatmap()
    {
        Graphics.Blit(rawTex, blurTemp, blurMat);     // Step 1: blur
        Graphics.Blit(blurTemp, finalHeatmap, colorMat); // Step 2: color
    }
    
    [Button]
    void Start()
    {
        rawTex = new Texture2D(textureSize, textureSize, TextureFormat.RFloat, false);
        ClearTexture();
    }

    public void ClearTexture()
    {
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
                rawTex.SetPixel(x, y, Color.black);
        }
        rawTex.Apply();
    }

    public void AddSensorValue(Vector2 uv, float value)
    {
        int x = Mathf.RoundToInt(uv.x * textureSize);
        int y = Mathf.RoundToInt(uv.y * textureSize);

        rawTex.SetPixel(x, y, new Color(value, value, value));
    }

    public void Apply() => rawTex.Apply();

    /// <summary>
    /// //////////////////////////////////////////////////////
    /// </summary>

    public Vector2 pos;
    public float temperatureValue;
    
    [Button]
    private void Test()
    {
        AddSensorValue(pos, temperatureValue);
        Apply();
    }
}