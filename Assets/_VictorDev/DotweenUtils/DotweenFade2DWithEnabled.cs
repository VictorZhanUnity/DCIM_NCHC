using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _VictorDev.DoTweenUtils
{
    /// Enabled動畫控制器
    public class DotweenFade2DWithEnabled : MonoBehaviour
    {
        #region [Components]

        [SerializeField] private bool isOnEnabled = true;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private bool isRandomDelay = true;
        [SerializeField] private float delay = 0.3f;
        [SerializeField] private float delay_Start = 0f;
        [SerializeField] private Ease ease = Ease.OutQuad;
        [Header(">>> 是否移動")]
        [SerializeField] private bool isDoMove = false;
        [SerializeField] private Vector3 fromPosValue = Vector3.zero;
        [Header(">>> 是否縮放")]
        [SerializeField] private bool isDoScale = false;
        [SerializeField] private float fromScaleValue = 1f;

        [Header(">>> 動畫目標對像(若為空則自動指向本身")]
        [SerializeField] private Transform targetTrans;
        private Vector3? originalPos { get; set; } = null;
        private Vector3? originalScale { get; set; } = null;
        public CanvasGroup canvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();
        [NonSerialized] private CanvasGroup _canvasGroup;
        #endregion

        [Header(">>> [Event] 當動畫結束時Invoke")]
        public UnityEvent onAnimateFinished = new UnityEvent();
        [Header(">>> [Event] OnEabled時Invoke")]
        public UnityEvent onEnabledEvent = new UnityEvent();
        [Header(">>> [Event] OnDisabled時Invoke")]
        public UnityEvent onDisabledEvent = new UnityEvent();

        private RectTransform targetRectTrans;
        
        private void OnEnable()
        {
            onEnabledEvent?.Invoke();
            if(isOnEnabled) ToShow();
        }

        [ContextMenu("- 播放Dotween動畫")]
        public void ToShow()
        {
            DOTween.Kill(targetRectTrans);

            if (targetTrans == null) targetTrans = transform;
            targetRectTrans = targetTrans as RectTransform;

            originalPos ??= targetRectTrans.localPosition;
            originalScale ??= targetRectTrans.localScale;
            if (targetRectTrans.TryGetComponent(out CanvasGroup cg) == false)
            {
                cg = targetRectTrans.gameObject.AddComponent<CanvasGroup>();
            }
            Vector3 fromPos = (originalPos ?? Vector3.zero) + fromPosValue;
            float targetDelay = delay_Start + (isRandomDelay ? Random.Range(0, delay) : delay);
            cg.alpha = 0;
            void CheckAlpha() => cg.interactable = cg.blocksRaycasts = cg.alpha == 1;
            CheckAlpha();
            cg.DOFade(1, duration).From(0).SetEase(ease).SetDelay(targetDelay).OnUpdate(CheckAlpha).OnComplete(()=>onAnimateFinished?.Invoke()).SetTarget(targetRectTrans);

            if (isDoMove) targetRectTrans.DOLocalMove(originalPos ?? Vector3.zero, duration).From(fromPos).SetEase(ease).SetDelay(targetDelay).SetTarget(targetRectTrans);
           // if (isDoMove) targetRectTrans.DOAnchorPos(originalPos ?? Vector3.zero, duration).From(fromPos).SetEase(ease).SetDelay(targetDelay).SetTarget(targetRectTrans);
            if (isDoScale) targetRectTrans.DOScale(originalScale ?? Vector3.zero, duration).From(new Vector3(fromScaleValue, fromScaleValue, fromScaleValue)).SetEase(ease).SetDelay(targetDelay).SetTarget(targetRectTrans);
            
            gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            DOTween.Kill(targetTrans);
            onDisabledEvent?.Invoke();
        }
    }
}
