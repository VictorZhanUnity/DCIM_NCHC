using _VictorDev.ApiExtensions;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.ImageUtils
{
    [RequireComponent(typeof(Image))]
    public class ImageDoTween : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private Image image;
        [Foldout("[設定]"), SerializeField] private float duration = 0.5f, delay;
        [Foldout("[設定]"), SerializeField] private Ease ease = Ease.OutQuad;

        private Tween tween;

        #endregion

        public float fillAmount
        {
            set
            {
                tween.TryToKill();
                tween = image.DOFillAmount(value, duration).SetEase(ease).SetDelay(delay);
            }
        }

        private void OnValidate()
        {
            if (image == null) image = GetComponent<Image>();
        }

        private void OnDisable() => tween.TryToKill();
    }
}