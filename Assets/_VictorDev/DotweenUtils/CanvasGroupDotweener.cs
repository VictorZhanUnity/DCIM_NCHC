using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.DoTweenUtils
{
    /// CanvasGroup進行DOFade
    public class CanvasGroupDotweener : _DotweenerBase
    {
        public void SetEnabled(bool isEnabled) => alpha = isEnabled ? valueEnabled : valueDisabled;
        
        public float alpha
        {
            set
            {
                if (Application.isPlaying)
                {
                    Cg.interactable = Cg.blocksRaycasts = false || !isInteractableByTween;
                    SetDelayAndEase(Cg.DOFade(value, Duration)).OnComplete(() =>
                    {
                        Cg.interactable = Cg.blocksRaycasts =
                            Mathf.Approximately(Cg.alpha, 1) || !isInteractableByTween;
                    });
                }
                else Cg.alpha = value;
            }
        }

        [Foldout("設定Enabled/Disabled")] [SerializeField]
        private float valueEnabled = 1, valueDisabled = 0.05f;

        [Foldout("設定")] [SerializeField] private bool isInteractableByTween;
        private CanvasGroup Cg => _cg ??= GetComponent<CanvasGroup>();
        [NonSerialized] private CanvasGroup _cg;
    }
}