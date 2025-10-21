using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace VictorDev.ColorUtils
{
    /// 設定UI組件的Color切換
    public class UiColorChanger : MonoBehaviour
    {
        #region Variables
        [Label("[Color設定]")] [SerializeField] private Color[] colors;
        [Label("[Image/TextmeshProUGUI]")] [SerializeField]
        private List<Graphic> targets;
        [Foldout("[設定]"), Label("[可選] - 自動綁定Toggle.isOn判斷")] [SerializeField]
        private Toggle toggleTarget;
        #endregion

        private void OnEnable() => toggleTarget?.onValueChanged.AddListener(ChangeColor);
        private void OnDisable() => toggleTarget?.onValueChanged.RemoveListener(ChangeColor);

        /// 設置顏色Index (true:0/false:1)
        public void ChangeColor(bool isOn)
        {
            if (colors.Length < 2)
                Debug.LogWarning($"Colors count is less than 2!", this);
            else
                ChangeColor(isOn ? 0 : 1);
        }

        public void ChangeColor(int index)
        {
            if (toggleTarget != null && toggleTarget.isOn) return;
            targets.ForEach(target => target.color = colors[index]);
        }
    }
}