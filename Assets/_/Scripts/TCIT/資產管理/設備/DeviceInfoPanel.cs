using System.Collections.Generic;
using _VictorDev.CameraUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資訊面板
    public class DeviceInfoPanel: MonoBehaviour
    {
        [Label("[資料項]"), SerializeField] private DeviceRevitAssetData deviceRevitAssetData;
        [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;

        public void ReceiveData(DeviceRevitAssetData data)
        {
            deviceRevitAssetData = data;
            ToClose();
            UpdateUI();
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(deviceRevitAssetData, txtComps);
        }

        public void ToClose()
        {
            gameObject.SetActive(false);
        }
        
        public void CameraToPosition() => RTSCameraController.CameraToPosition(deviceRevitAssetData.Model);
    }
}