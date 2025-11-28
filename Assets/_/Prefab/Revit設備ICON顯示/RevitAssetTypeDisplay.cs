using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    /// 顯示Revit資料類型ICON
    public class RevitAssetTypeDisplay : MonoBehaviour
    {
        #region Variables

        [SerializeField] private EnumRevitAssetKind revitAssetKind;
        [Label("[Image組件]"), SerializeField] private List<Image> icons;

        #endregion

        public void SetDeviceKind(EnumRevitAssetKind value)
        {
            revitAssetKind = value;
            UpdateUI();
        }

        private void UpdateUI() => icons.ForEach(icon =>
        {
            icon.gameObject.SetActive(icon.name.Equals(revitAssetKind.ToString(), StringComparison.OrdinalIgnoreCase));
        });

        private void Reset() => icons = GetComponentsInChildren<Image>(true).ToList();

        private void OnValidate() => UpdateUI();
    }
}