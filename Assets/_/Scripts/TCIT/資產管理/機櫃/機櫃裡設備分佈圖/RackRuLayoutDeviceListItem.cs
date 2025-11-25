using System;
using System.Collections.Generic;
using _VictorDev.MediatorUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃Layout列表裡的設備ListItem
    public class RackRuLayoutDeviceListItem : MonoBehaviour
    {
        #region Varibles
        [Foldout("[Event]")] public UnityEvent<RackRuLayoutDeviceListItem> onItemSelectedEvent;
        
        [Foldout("[組件]"), SerializeField] private Toggle toggle;
        [Foldout("[組件]"), SerializeField] private RevitAssetDataHolder revitAssetDataHolder;
        [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;
        #endregion

        public DeviceRevitAssetData DeviceRevitData => revitAssetDataHolder.DeviceRevitData;
        
        public void ReceiveAssetData(DeviceRevitAssetData deviceRevitAssetData)
        {
            revitAssetDataHolder.ReceiveAssetData(deviceRevitAssetData);
            UpdateUI();
        }

        private void UpdateUI() => TextHelper.SetParamsToTxtComps(revitAssetDataHolder.DeviceRevitData, txtComps);

        public void SetToggleIsOn(bool isOn) => toggle.isOn = isOn;

        #region Initialized
        private void OnEnable() => toggle.onValueChanged.AddListener(OnToggleValueChangedHandler);
        private void OnDisable() => toggle.onValueChanged.RemoveListener(OnToggleValueChangedHandler);
        private void OnToggleValueChangedHandler(bool isOn)
        {
            if(isOn) onItemSelectedEvent?.Invoke(this);
        }

        private void Start() => OnValidate();

        private void OnValidate()
        {
            toggle ??= GetComponentInChildren<Toggle>(true);
            if(toggle != null) toggle.group = GetComponentInParent<ToggleGroup>(true);
            revitAssetDataHolder ??= GetComponent<RevitAssetDataHolder>();
            if(txtComps.Count == 0) txtComps = ObjectHelper.FindChildrenByClass<TextDotweener, TextDotweener>(transform, true
                , "TxtDeviceName", "TxtDeviceCode", "TxtDeviceKind");
        }
        #endregion
    }
}
