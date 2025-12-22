using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料管理器
    public class RevitAssetDataManager : JsonDataManagerParent<List<RackRevitAssetData>>
    {
        private bool IsHaveData => Data.ClearMissingTargets().Count > 0;

        [Button, ShowIf(nameof(IsHaveData))]
        private void CopyRackDeviceCodesToClipboard()
        {
            string result = "";
            Data.ForEach(rackData => result += $"\"{rackData.deviceCode}\", \n");
            Debug.Log($"全機櫃deviceCode已複製至Clipboard");
            GUIUtility.systemCopyBuffer = result;
        }
    }
}