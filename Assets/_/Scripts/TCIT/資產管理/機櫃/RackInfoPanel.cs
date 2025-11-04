using System.Collections.Generic;
using _VictorDev.CameraUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃資訊面板
    public class RackInfoPanel: MonoBehaviour
    {
        #region Variables
        [Label("[資料項]"), SerializeField] private RackRevitAssetData rackRevitAssetData;
        [Foldout("[Event] - ReceiveData")] public UnityEvent onReceiveDataEvent;
        [Foldout("[Event] - OnClose")] public UnityEvent onCloseEvent;
        [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;
        #endregion

        public void ReceiveData(RackRevitAssetData data)
        {
            onReceiveDataEvent?.Invoke();
            rackRevitAssetData = data;
            UpdateUI();
            gameObject.SetActive(true);
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
        }
        
        public void ToClose()
        {
            onCloseEvent?.Invoke();
           gameObject.SetActive(false);
        }
        
        public void CameraToPosition() => RTSCameraController.CameraToPosition(rackRevitAssetData.Model);
    }
}