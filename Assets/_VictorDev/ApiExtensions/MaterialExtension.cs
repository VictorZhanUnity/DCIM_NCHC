using UnityEngine;

namespace VictorDev.ApiExtensions
{
    /// 原API類別功能擴充
    public static class MaterialExtension
    {
        /// [Extension] - 全部替換成指定material
        public static Material[] ToCopyWithChangeMaterials(this Material[] materials, Material material)
        {
            if (materials.IsNullOrEmptyWithLog(nameof(materials)) ||
                material.IsNullOrEmptyWithLog(nameof(material))) return null;
         
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            
            return materials;
        }

        /// [Extension] - 設定Emission
        public static Material SetEmission(this Material target, bool isEnabled, Color color = default,
            float intensity = 1.5f)
        {
            if (isEnabled)
            {
                target.EnableKeyword("_EMISSION");
                Color finalColor = color.linear * Mathf.GammaToLinearSpace(intensity);
                target.SetColor("_EmissionColor", finalColor);
            }
            else target.DisableKeyword("_EMISSION");

            return target;
        }
    }
}