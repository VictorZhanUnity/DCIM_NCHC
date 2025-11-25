using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.Configs;
using DG.Tweening;
using UnityEngine;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.ApiExtensions
{
    /// [Extended] 原API類別功能擴充
    public static class TransformExtension
    {
        /// 目標物件的階層路徑
        public static string GetHierarchyPath(this Transform self)
        {
            string path = self.name;
            while (self.parent != null)
            {
                self = self.parent;
                path = self.name + "/" + path;
            }
            return path;
        }
        
        #region 依關鍵字搜尋子物件
        /// 搜尋子物件(有實作MeshRenderer)，名稱(包含/不包含)關鍵字
        public static List<Transform> FindChildrenByKeywords(this Transform self, EnumSearchType searchType = EnumSearchType.Include, params string[] keywords) 
            => FindChildrenByKeywords<MeshRenderer>(self, searchType, keywords);
        /// 搜尋子物件(有實作T類別)，名稱(包含/不包含)關鍵字
        public static List<Transform> FindChildrenByKeywords<T>(this Transform self, EnumSearchType searchType = EnumSearchType.Include, params string[] keywords)
         where T : Component
        {
            return self.GetComponentsInChildren<Transform>(includeInactive: true).Where(t => 
                    t != self && t.GetComponent<T>() != null && IsMatch(t.name, searchType, keywords))
                    .OrderBy(t=> t.name).ToList();
        }
        
        private static bool IsMatch(string name, EnumSearchType searchType, string[] keywords)
        {
            bool containsAny = keywords.Any(key =>
                name.Contains(key, StringComparison.OrdinalIgnoreCase));

            return searchType switch
            {
                EnumSearchType.Include => containsAny,      // 只要包含任一關鍵字即可
                EnumSearchType.Exclude => !containsAny,     // 不得包含任何關鍵字
                _ => false
            };
        }
        #endregion
        
        #region 從父物件中TryGetComponent
        
        /// 嘗試從父物件中取得指定型別的元件（適用於 Component 呼叫）。(若持續往上抓到根物件為止)
        public static bool TryGetComponentInParent<T>(this Component componentRoot, out T component, bool includeInactive = false)
            where T : Component =>
            TryGetComponentInParent(componentRoot.gameObject, out component, includeInactive);
        
        /// 嘗試從父物件中取得指定型別的元件。(若持續往上抓到根物件為止)
        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component, bool includeInactive = false)
            where T : Component
        {
            component = gameObject.GetComponentInParent<T>(includeInactive);
            return component != null;
        }
      
        #endregion

        #region 從子物件中TryGetComponent
        /// 嘗試從子物件中取得指定型別的元件（適用於 Component 呼叫）。(若持續往上抓到根物件為止)
        public static bool TryGetComponentInChildren<T>(this Component componentRoot, out T component, bool includeInactive = false)
            where T : Component =>
            TryGetComponentInChildren(componentRoot.gameObject, out component, includeInactive);
        /// 嘗試從子物件中取得指定型別的元件。(若持續往上抓到根物件為止)
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component, bool includeInactive = false)
            where T : Component
        {
            component = gameObject.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }
        #endregion

        #region 將Pivot設為模型中心點的相關處理
        /// 取得模型物件所有Mesh結合起來的正中心點
        public static Vector3 GetMeshCenter(this Transform target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return target.position;

            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            return bounds.center;
        }
        
        /// 以模型中心點設置Position
        public static void SetCenterPosition(this Transform target, Vector3 newCenterPos)
        {
            Vector3 meshCenter = target.GetMeshCenter();
            Vector3 offset = target.position - meshCenter;
            target.position = newCenterPos + offset;
        }

        /// 以模型中心點設置Rotation
        public static void SetCenterRotation(this Transform target, Quaternion newRotation)
        {
            Vector3 center = target.GetMeshCenter();
            Vector3 pivotOffset = target.position - center;
            // 以幾何中心為基準旋轉
            target.rotation = newRotation;
            target.position = center + target.rotation * pivotOffset;
        }
        #endregion
        
        /// 跟隨Target物件
        public static void FollowTarget(this Transform transform, Transform target, float lerpDuration = 0f,
            float delay = 0f, Ease ease = Ease.Linear)
        {
            if (target.TryGetComponent(out MeshRenderer targetMeshRenderer))
            {
                transform.DOMove(targetMeshRenderer.bounds.center, lerpDuration).SetEase(ease).SetDelay(delay);
            }
            else
            {
                Debug.LogError("Target doesn't have a MeshRenderer");
            }
        }
        
        
        /// [Extension] - Lerp移動
        public static Coroutine ToLerpMove(this Transform self, MonoBehaviour runner,
            Vector3 targetPosition, float duration, Func<float, float> easingFunction,
            Action onComplete = null)
        {
            easingFunction ??= EaseForLerp.Linear;
            return runner.StartCoroutine(LerpRoutine());

            IEnumerator LerpRoutine()
            {
                Vector3 start = self.position;
                float time = 0;

                while (time < duration)
                {
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float easedT = easingFunction(t);
                    self.position = Vector3.Lerp(start, targetPosition, easedT);
                    yield return null;
                }

                self.position = targetPosition; // 保證精準收尾
                onComplete?.Invoke();
            }
        }

        #region MeshRenderer.material處理

        // 紀錄原本材質
        private static readonly Dictionary<Transform, Material[]> OriginalMaterials = new();
        
        /// [Extension] - 替換MeshRender.material為指定材質，並儲存原材質與Extension裡
        public static Transform ChangeMeshMaterialAndRecord(this Transform self, Material newMaterial, bool isIncludeChildren = true)
        {
            if (self.TryGetComponent(out MeshRenderer renderer))
            {
                // 若還沒記錄過才存
                if (!OriginalMaterials.ContainsKey(self)) OriginalMaterials[self] = renderer.materials;
                
                // 替換為指定材質
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    newMaterials[i] = newMaterial;
                }
                renderer.materials = newMaterials;
            }
            else Debug.LogWarning($"{self.name} has no MeshRenderer component", nameof(TransformExtension), EmojiEnum.Warning);
            return self;
        }
        /// [Extension] - 還原先前記錄的材質
        public static Transform RestoreMeshMaterial(this Transform self)
        {
            if (OriginalMaterials.TryGetValue(self, out Material[] materialsResult))
            {
                if (self.TryGetComponent(out MeshRenderer renderer))
                {
                    renderer.materials = materialsResult;
                    OriginalMaterials.Remove(self);
                }
            }
            else Debug.LogWarning($"{self.name} has no recorded original materials to restore.");
            return self;
        }
        #endregion        
    }
    
    
    public enum EnumEaseType
    {
        Linear, EaseInQuad, EaseOutQuad, EaseInOutQuad, EaseInCubic, EaseOutCubic, EaseInOutCubic
    }

    public static class EasingResolver
    {
        public static Func<float, float> GetEase(EnumEaseType type)
        {
            return type switch
            {
                EnumEaseType.Linear => EaseForLerp.Linear,
                EnumEaseType.EaseInQuad => EaseForLerp.EaseInQuad,
                EnumEaseType.EaseOutQuad => EaseForLerp.EaseOutQuad,
                EnumEaseType.EaseInOutQuad => EaseForLerp.EaseInOutQuad,
                EnumEaseType.EaseInCubic => EaseForLerp.EaseInCubic,
                EnumEaseType.EaseOutCubic => EaseForLerp.EaseOutCubic,
                EnumEaseType.EaseInOutCubic => EaseForLerp.EaseInOutCubic,
                _ => EaseForLerp.Linear
            };
        }
    }
    
    public static class EaseForLerp
    {
        public static float Linear(float t) => t;

        public static float EaseInQuad(float t) => t * t;

        public static float EaseOutQuad(float t) => 1 - (1 - t) * (1 - t);

        public static float EaseInOutQuad(float t) =>
            t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;

        public static float EaseInCubic(float t) => t * t * t;

        public static float EaseOutCubic(float t) => 1 - Mathf.Pow(1 - t, 3);

        public static float EaseInOutCubic(float t) =>
            t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

        public static float EaseInSine(float t) => 1 - Mathf.Cos((t * Mathf.PI) / 2);

        public static float EaseOutSine(float t) => Mathf.Sin((t * Mathf.PI) / 2);

        public static float EaseInOutSine(float t) => -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

        public static float EaseInExpo(float t) => t == 0 ? 0 : Mathf.Pow(2, 10 * (t - 1));

        public static float EaseOutExpo(float t) => t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);

        public static float EaseInOutExpo(float t) =>
            t == 0 ? 0 : t == 1 ? 1 :
            t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 :
            (2 - Mathf.Pow(2, -20 * t + 10)) / 2;

        public static float EaseInBack(float t, float s = 1.70158f) =>
            s * t * t * ((s + 1) * t - s);

        public static float EaseOutBack(float t, float s = 1.70158f)
        {
            t -= 1;
            return 1 + s * t * t * ((s + 1) * t + s);
        }

        public static float EaseInOutBack(float t, float s = 1.70158f * 1.525f) =>
            t < 0.5f
                ? (Mathf.Pow(2 * t, 2) * ((s + 1) * 2 * t - s)) / 2
                : (Mathf.Pow(2 * t - 2, 2) * ((s + 1) * (t * 2 - 2) + s) + 2) / 2;
    }

}