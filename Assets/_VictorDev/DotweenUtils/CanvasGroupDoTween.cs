using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.DoTweenUtils
{
    /// CanvasGroup進行DOFade
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupDoTween : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event] Tween時Invoke")] public UnityEvent<bool> isEnabledEvent;
        [Foldout("[Event] Tween時Invoke")] public UnityEvent onTweenStartEvent, onTweenEndEvent;
        [Foldout("設定"), SerializeField] private float alphaOnEnabled = 1, alphaOnDisabled = 0.05f;
        [Foldout("設定"), SerializeField] private float duration = 0.5f, delay = 0f;
        [Foldout("設定"), SerializeField] private Ease ease = Ease.OutQuad;
        [Foldout("設定"), SerializeField] private CanvasGroup canvasGroup;
        public bool IsOn { get; private set; }

        #endregion

        private void Awake() => OnDisable();

        private void OnDisable() => SetEnabled(false);

        public void SetEnabled(bool isEnabled)
        {
            onTweenStartEvent?.Invoke();
            IsOn = isEnabled;
            float targetAlpha = IsOn ? alphaOnEnabled : alphaOnDisabled;
            canvasGroup.DOFade(targetAlpha, duration).SetEase(ease).SetDelay(delay).OnUpdate(OnUpdateHandler).OnComplete(OnCompleteHandler);
        }

        private void OnCompleteHandler()
        {
           isEnabledEvent?.Invoke(IsOn);
           onTweenEndEvent?.Invoke();
        }

        private void OnUpdateHandler()
        {
            bool isInteractable = Mathf.Approximately(canvasGroup.alpha, 1f);
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
        }

        private void OnValidate() => canvasGroup ??= GetComponent<CanvasGroup>();
    }
}