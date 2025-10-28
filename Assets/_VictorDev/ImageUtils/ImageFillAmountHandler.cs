using System.Collections.Generic;
using System.Linq;
using VictorDev.ColorUtils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
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
            if (Application.isPlaying)
            {
                imageFillTarget.DOFillAmount(receivePercentageValue, duration).SetEase(Ease.OutQuad);
            }
            else
            {
                imageFillTarget.fillAmount = receivePercentageValue;
            }

            if (isLerpColor)
            {
                ColorSet colorResult = colorLevels.FirstOrDefault(colorSet=> receivePercentageValue <= colorSet.threshold);
                if (Application.isPlaying)
                {
                    imageFillTarget.DOColor(colorResult.color, duration).SetEase(Ease.OutQuad);
                    doColorTargets.ForEach(target=> target.DOColor(colorResult.color, duration).SetEase(Ease.OutQuad));
                }
                else
                {
                    imageFillTarget.color = colorResult.color;
                    doColorTargets.ForEach(target=> target.color = colorResult.color);
                }
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
        [Foldout("[設定]"), SerializeField] private Image imageFillTarget;
        [Foldout("[設定]"), SerializeField] private List<Graphic> doColorTargets;
        #endregion

        private void OnValidate()
        {
            imageFillTarget ??= GetComponent<Image>();
            imageFillTarget.type = Image.Type.Filled;
            
            colorLevels = colorLevels?.OrderBy(colorSet=> colorSet.threshold).ToList();
        }
      
    }
}