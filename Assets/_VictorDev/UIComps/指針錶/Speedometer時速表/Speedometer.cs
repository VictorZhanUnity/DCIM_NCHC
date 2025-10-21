using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.Frameworks;

namespace VictorDev.UIComps
{
    public class Speedometer : MonoBehaviour
    {
        [ReadOnly, SerializeField] private float currentValue;
        public float CurrentValue => currentValue;
        
        [Foldout("[設定]"), SerializeField] private float maxValue = 100f; // 錶盤最大值
        [Foldout("[設定]"), SerializeField] private float minDotAngle = 0f; // 指針對應0值的角度
        [Foldout("[設定]"), SerializeField] private float maxDotAngle = -180f; // 指針對應最大值的角度
        [Foldout("[設定]"), SerializeField] private float duration = 0.5f; // 指針對應最大值的角度
        [Foldout("[設定]"), SerializeField] private RectTransform needleDot; // 指針

        [Foldout("[設定]"), SerializeField, Label("進度條(選填)")]
        private ImageFillAmountHandler imageProgressbar;

        public void SetValue(float value)
        {
            currentValue = value;
            UpdateUI();
        }

        private void UpdateUI()
        {
            float percentage = currentValue / maxValue;
            percentage = Mathf.Clamp01(percentage);
            imageProgressbar?.DoFillAmount(percentage);

            float dotAngle = minDotAngle + percentage * (maxDotAngle - minDotAngle);
            needleDot.DORotate(new Vector3(0, 0, dotAngle), duration).SetEase(Ease.OutQuad);
        }
    }
}