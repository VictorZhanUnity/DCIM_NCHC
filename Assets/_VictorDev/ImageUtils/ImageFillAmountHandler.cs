using System.Collections.Generic;
using System.Linq;
using VictorDev.ColorUtils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace VictorDev.Frameworks
{
    /// 設定Image Fill Amount與階級變色
    [RequireComponent(typeof(Image))]
    public class ImageFillAmountHandler : MonoBehaviour
    {
        public void DoFillAmount(float value)
        {
            receivePercentageValue = value / (isValueOf01? 1: 100f);
            image.DOFillAmount(receivePercentageValue, duration).SetEase(Ease.OutQuad);

            if (isLerpColor)
            {
                ColorSet colorResult = colorLevels.FirstOrDefault(colorSet=> receivePercentageValue <= colorSet.threshold);
                image.DOColor(colorResult.color, duration).SetEase(Ease.OutQuad);
            }
        }

        #region Variables
        [Label("[資料項] - 接收到的數值"), ReadOnly, SerializeField] private float receivePercentageValue;
        [Foldout("[設定]"), SerializeField] private bool isValueOf01 = true;
        [Foldout("[設定]"), SerializeField] private float duration = 0.5f;
        [Foldout("[設定]"), SerializeField] private bool isLerpColor = true;
        [Foldout("[設定]"), SerializeField, ShowIf(nameof(isLerpColor))]
        private List<ColorSet> colorLevels = new ()
        {
            new ColorSet(0.4f, ColorHelper.HexToColor(0x7BFF69)),
            new ColorSet(0.6f, ColorHelper.HexToColor(0xFFF532)),
            new ColorSet(0.8f, ColorHelper.HexToColor(0xEF701A)),
            new ColorSet(1f, ColorHelper.HexToColor(0x640000)),
        };
        [Foldout("[設定]"), SerializeField] private Image image;
        #endregion

        private void OnValidate()
        {
            image ??= GetComponent<Image>();
            image.type = Image.Type.Filled;
        }
      
    }
}