using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Debug = VictorDev.DebugUtils.Debug;

namespace VictorDev.ApiExtensions
{
    /// [Extended] 原API類別功能擴充
    public static class TransformExtension
    {
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