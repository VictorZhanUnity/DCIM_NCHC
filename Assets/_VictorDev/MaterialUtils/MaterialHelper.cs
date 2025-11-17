using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VictorDev.MaterialUtils
{
    public static class MaterialHelper
    {
        #region Replace Material

        /// 存儲每個物件及其原始材質的字典 {物件Transform, 材質陣列}
        private static Dictionary<Transform, Material[]> _originalMaterials = new();

        /// 替換物件及其底下每層所有子物件的材質 {排除的對像(選填)}
        public static void ReplaceMaterialRecursively(Transform target, Material material,
            List<Transform> excludeTargets = null)
        {
            if (excludeTargets != null)
            {
                //當目標對像不在排除名單內時
                if (excludeTargets.Contains(target) == false)
                {
                    ReplaceMaterial(target, material);
                    // 遞迴處理所有子物件
                    foreach (Transform child in target)
                    {
                        ReplaceMaterialRecursively(child, material, excludeTargets);
                    }
                }
            }
            else
            {
                //當沒有排除名單時，直接替換
                ReplaceMaterial(target, material);
                // 遞迴處理所有子物件
                foreach (Transform child in target)
                {
                    ReplaceMaterialRecursively(child, material);
                }
            }
        }

        /// 替換Targets(陣列)為指定材質
        public static void ReplaceMaterial(List<Transform> targets, Material replaceMaterial) =>
            targets.ForEach(target => ReplaceMaterial(target, replaceMaterial));

        /// 替換Targets(陣列)為指定材質
        public static void ReplaceMaterial(Transform target, Material replaceMaterial)
        {
            if (target.TryGetComponent(out Renderer render))
            {
                // 如果尚未保存原始材質，將它的材質陣列存儲到字典中
                _originalMaterials.TryAdd(target, render.sharedMaterials);

                // 進行材質替換
                if (render.sharedMaterials.Length > 1)
                {
                    // 如果有多個材質，建立新的材質陣列
                    Material[] newMaterials = new Material[render.sharedMaterials.Length];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        // 替換為指定的材質
                        newMaterials[i] = replaceMaterial;
                    }

                    // 套用新的材質陣列
                    render.materials = newMaterials;
                }
                else
                {
                    // 如果只有一個材質，直接替換
                    render.material = replaceMaterial;
                }
            }
        }

        #endregion

        #region Restore Material

        /// 復原全部對像的原始材質
        public static void RestoreAllMaterials()
        {
            foreach (var kvp in _originalMaterials)
            {
                RestoreMaterial(kvp.Key);
            }
        }

        /// 復原對像(陣列)的原始材質，並從Dictionary裡移除
        public static void RestoreMaterial(List<Transform> targets) =>
            targets.ForEach(target => RestoreMaterial(target));

        /// 復原對像的原始材質，並從Dictionary裡移除
        public static void RestoreMaterial(Transform target)
        {
            if (_originalMaterials.TryGetValue(target, out Material[] materials))
            {
                if (target.TryGetComponent(out Renderer render))
                {
                    render.materials = materials;
                }
            }
        }
        #endregion

        #region 設定Material屬性

        /// 将材质设置为透明模式
        public static void SetTransparentMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 3); // 设置模式为 Transparent
            targetMaterial.SetOverrideTag("RenderType", "Transparent");
            targetMaterial.EnableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            targetMaterial.SetInt("_ZWrite", 0); // 关闭深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Transparent; // 设置渲染队列为透明层
        }

        /// 将材质设置为不透明模式
        public static void SetOpaqueMode(Material targetMaterial)
        {
            targetMaterial.SetFloat("_Mode", 0); // 设置模式为 Opaque
            targetMaterial.SetOverrideTag("RenderType", "Opaque");
            targetMaterial.DisableKeyword("_ALPHABLEND_ON");
            targetMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
            targetMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
            targetMaterial.SetInt("_ZWrite", 1); // 开启深度写入
            targetMaterial.renderQueue = (int)RenderQueue.Geometry; // 设置渲染队列为几何层
        }

        #endregion
    }
}