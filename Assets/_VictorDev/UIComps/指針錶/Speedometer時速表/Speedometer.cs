using System;
using _VictorDev.MediatorUtils;
using _VictorDev.ImageUtils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.UIComps
{
    public class Speedometer : MonoBehaviour
    {
        #region Variables
        [ReadOnly, SerializeField] private float currentValue;
        public float CurrentValue => currentValue;
        
        [Foldout("[設定]"), SerializeField] private float maxValue = 100f; // 錶盤最大值
        [Foldout("[設定]"), SerializeField] private float minDotAngle = 0f; // 指針對應0值的角度
        [Foldout("[設定]"), SerializeField] private float maxDotAngle = -180f; // 指針對應最大值的角度
        [Foldout("[設定]"), SerializeField] private float duration = 0.5f, delay=0; // 指針對應最大值的角度
        [Foldout("[設定]"), SerializeField] private RectTransform needleDot; // 指針

        [Foldout("[設定]"), SerializeField, Label("進度條(選填)")]
        private ImageFillAmountHandler imageProgressbar;
        #endregion


        private void OnValidate()
        {
            imageProgressbar.SetDuration(duration, delay);
        }

        private void OnEnable()
        {
            imageProgressbar.SetValue(0);
            imageProgressbar?.DoFillAmount(percentage);
            needleDot.DORotate(new Vector3(0, 0, dotAngle), duration).From(Vector3.zero).SetEase(Ease.OutQuad);
        }

        public void SetValue(float value)
        {
            currentValue = value;
            UpdateUI();
        }
        public void SetMaxValue(float value)
        {
            maxValue = value;
            UpdateUI();
        }

        private void UpdateUI()
        {
            percentage = Mathf.Clamp01(currentValue / maxValue);
            imageProgressbar?.DoFillAmount(percentage);

            dotAngle = minDotAngle + percentage * (maxDotAngle - minDotAngle);
            if (Application.isPlaying)
            {
                needleDot.DORotate(new Vector3(0, 0, dotAngle), duration).SetEase(Ease.OutQuad).SetDelay(delay);
            }
            else
            {
                needleDot.rotation = Quaternion.Euler(0, 0, dotAngle);
            }
        }

        private float dotAngle = 0, percentage = 0;

    }
}