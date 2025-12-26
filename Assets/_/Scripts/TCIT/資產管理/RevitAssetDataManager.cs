using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料管理器
    public class RevitAssetDataManager : JsonDataManagerParent<List<RackRevitAssetData>>
    {
        #region Variables

        [Foldout("[Event] 接收到資料時")] public UnityEvent onReceiveDataEvent;
        private bool IsHaveData => Data != null && Data.ClearMissingTargets().Count > 0;
        #endregion

        [Button, ShowIf(nameof(IsHaveData))]
        private void CopyRackDeviceCodesToClipboard()
        {
            string result = "";
            Data.ForEach(rackData => result += $"\"{rackData.deviceCode}\", \n");
            Debug.Log($"全機櫃deviceCode已複製至Clipboard");
            GUIUtility.systemCopyBuffer = result;
        }

        protected override void BeforeInvokeData()
        {
            onReceiveDataEvent?.Invoke();
        }
    }
}