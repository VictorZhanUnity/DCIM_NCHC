using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DoTweenUtils;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備 資訊面板
    public class DeviceAssetDataPanel : MonoBehaviour
    {
        #region Variables

        [Label("[資料項 - DeviceRevitAssetData]"), SerializeField] private DeviceRevitAssetData deviceRevitAssetData;
        [Label("[Txt組件]"), SerializeField] private List<TextDotweener> txtComps;

        #endregion

        public void SetDeviceRevitAssetData(DeviceRevitAssetData data)
        {
            deviceRevitAssetData = data;
            UpdateUI();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(deviceRevitAssetData, txtComps);
        }

        [Button]
        private void FindTxtComponents()
        {
            txtComps = transform.GetComponentsInChildren<TextDotweener>().ToList();
            txtComps = txtComps.FilterByNameForKeywords(EnumSearchType.Include, "Txt");
        }
    }
}