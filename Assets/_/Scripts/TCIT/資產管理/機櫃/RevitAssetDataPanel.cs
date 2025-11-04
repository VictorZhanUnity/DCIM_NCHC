using System.Collections.Generic;
using _VictorDev.CameraUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃/設備 資訊面板
    public class RevitAssetDataPanel : MonoBehaviour
    {
        #region Variables
        [Label("[資料項]"), SerializeField, ShowIf(nameof(isRackRevitData))] private RackRevitAssetData rackRevitAssetData;
        [Label("[資料項]"), SerializeField, ShowIf(nameof(isDeviceRevitData))] private DeviceRevitAssetData deviceRevitAssetData;

        private bool isRackRevitData, isDeviceRevitData;

        [Foldout("[Event] - ReceiveData")] public UnityEvent onReceiveDataEvent;
        [Foldout("[Event] - OnClose")] public UnityEvent onCloseEvent;
        [Foldout("[Event] - 收到RackRevitAssetData")] public UnityEvent onReceiveRackRevitDataEvent;
        [Foldout("[Event] - 收到DeviceRevitAssetData")] public UnityEvent onReceiveDeviceRevitDataEvent;
        [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;
        #endregion

        public void ReceiveData(RevitAssetData data)
        {
            isRackRevitData = data is RackRevitAssetData;
            isDeviceRevitData = !isRackRevitData;
            
            if(isRackRevitData) rackRevitAssetData = data as RackRevitAssetData;
            else deviceRevitAssetData = data as DeviceRevitAssetData;
            
            onReceiveDataEvent?.Invoke();
            (isRackRevitData? onReceiveRackRevitDataEvent: onReceiveDeviceRevitDataEvent)?.Invoke();
            
            UpdateUI();
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            if(isRackRevitData) TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
            else TextHelper.SetParamsToTxtComps(deviceRevitAssetData, txtComps);
        }

        public void ToClose()
        {
            onCloseEvent?.Invoke();
            gameObject.SetActive(false);
        }

        public void CameraToPosition() => RTSCameraController.CameraToPosition(isRackRevitData? rackRevitAssetData.Model: deviceRevitAssetData.Model);
    }
}