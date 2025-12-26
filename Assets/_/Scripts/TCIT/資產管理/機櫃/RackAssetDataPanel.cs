using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DoTweenUtils;
using _VictorDev.Framework.WebAPI;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃 資訊面板
    public class RackAssetDataPanel : MonoBehaviour
    {
        #region Variables

        [Label("[資料項 - RackRevitAssetData]"), SerializeField] private RackRevitAssetData rackRevitAssetData;
        [Label("[Txt組件]"), SerializeField] private List<TextDotweener> txtComps;
        [Foldout("[Event]")] public UnityEvent<List<DeviceRevitAssetData>> invokeContainerDevicesEvent;
        
        public WebAPICaller pduWebAPICaller;
        public WebAPICaller rtrhWebAPICaller;

        public ValueMediator valueWatt, valueWeight, valueHeightU;
        #endregion

        public UnityEvent onSetDataEvent;
        
        public void SetRackRevitAssetData(RackRevitAssetData data)
        {
            onSetDataEvent?.Invoke();
            
            rackRevitAssetData = data;
            UpdateUI();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            invokeContainerDevicesEvent?.Invoke(rackRevitAssetData.Containers);

            GetPDUData();
            GetRtRhData();
        }

        private void UpdateUI()
        {
            TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
            valueWatt.SetMaxValue(rackRevitAssetData.MaxWatt);
            valueWeight.SetMaxValue(rackRevitAssetData.MaxWeight);
            valueHeightU.SetMaxValue(rackRevitAssetData.MaxHeightU);
            valueWatt.SetValue(rackRevitAssetData.AvailableWatt);
            valueWeight.SetValue(rackRevitAssetData.AvailableWeight);
            valueHeightU.SetValue(rackRevitAssetData.AvailableHeightU);
        }

        private void GetPDUData()
        {
            pduWebAPICaller.UpdateFormData(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("deviceCode", rackRevitAssetData.deviceCode)
            });
            pduWebAPICaller.CallAPI();
        }

        public UnityEvent<float> pdu1ValueEvent, pdu2ValueEvent;
        public void ReceivePduData(string jsonString)
        {
            List<EnvironmentData> data = JsonConvert.DeserializeObject<List<EnvironmentData>>(jsonString);
            pdu1ValueEvent?.Invoke(data[0].tags[0].value);
            pdu2ValueEvent?.Invoke(data[0].tags[1].value);
        }
        
        private void GetRtRhData()
        {
            var jObj = JObject.Parse(rtrhWebAPICaller.SendBodyJson);
            jObj["deviceCode"][0] = rackRevitAssetData.deviceCode;
            rtrhWebAPICaller.SetBodyJson(jObj.ToString());
            rtrhWebAPICaller.CallAPI();
        }
        
        public UnityEvent<float> rtValueEvent, rhValueEvent;
        public void ReceiveRtRhData(string jsonString)
        {
            List<EnvironmentData> data = JsonConvert.DeserializeObject<List<EnvironmentData>>(jsonString);
            List<EnvironmentData.EnvironmentTag> result;
            result = data[0].tags.Where(t => t.tagDescription.Contains("溫度") && t.value >= 0).ToList();
            rtValueEvent?.Invoke(result.Count > 0? result[0].value: -1);
            
            result = data[0].tags.Where(t => t.tagDescription.Contains("濕度") && t.value >= 0).ToList();
            rhValueEvent?.Invoke(result.Count > 0? result[0].value: -1);
        }
        
        

        [Button]
        private void FindTxtComponents()
        {
            txtComps = transform.GetComponentsInChildren<TextDotweener>().ToList();
            txtComps = txtComps.FilterByNameForKeywords(EnumSearchType.Include, "Txt");
        }
    }
}