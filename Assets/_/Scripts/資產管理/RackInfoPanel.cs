using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using VictorDev.DoTweenUtils;
using VictorDev.TextUtils;

namespace VictorDev.TCIT
{
    /// 機櫃資訊面板
    public class RackInfoPanel: MonoBehaviour
    {
        [Label("[資料項]"), SerializeField] private RackAssetData rackAssetData;

        [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;

        public void ReceiveData(RackAssetData data)
        {
            gameObject.SetActive(false);
            rackAssetData = data;
            UpdateUI();
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(rackAssetData, txtComps);
        }
    }
}