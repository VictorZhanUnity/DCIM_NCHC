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
    /// 機櫃 資訊面板
    public class RackAssetDataPanel : MonoBehaviour
    {
        #region Variables

        [Label("[資料項]"), SerializeField] private RackRevitAssetData rackRevitAssetData;
        [Label("[Txt組件]"), SerializeField] private List<TextDotweener> txtComps;

        #endregion

        public void SetRackRevitAssetData(RackRevitAssetData data)
        {
            rackRevitAssetData = data;
            UpdateUI();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
        }

        [Button]
        private void FindTxtComponents()
        {
            txtComps = transform.GetComponentsInChildren<TextDotweener>().ToList();
            txtComps = txtComps.FilterByNameForKeywords(EnumSearchType.Include, "Txt");
        }
    }
}