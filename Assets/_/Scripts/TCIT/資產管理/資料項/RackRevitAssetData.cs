using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃資料
    [Serializable]
    public class RackRevitAssetData : RevitAssetData
    {
        /// 設備列表
        [JsonProperty]
        [field: SerializeField]
        public List<DeviceRevitAssetData> Containers { get; private set; }

        #region 額外增加的變數
        
        /// 機櫃編號
        public string RackNo;

        /// 設備已使用總電力
        public int UsageWatt => Containers.Sum(deviceData => deviceData.Watt);
        /// 設備已使用總重量
        public int UsageWeight => Containers.Sum(deviceData => deviceData.Weight);
        /// 設備已使用總U層數
        public int UsageHeightU => Containers.Sum(deviceData => deviceData.HeightU);
        
        /// 總電力 (+3000 For Demo)
        public int MaxWatt => Information.watt + 3000;
        /// 總負重
        public int MaxWeight => Information.weight + 3000;
        /// 總U層數
        public int MaxHeightU => Information.heightU;
        
        /// 可供電力
        public int AvailableWatt => Mathf.Clamp(MaxWatt - UsageWatt, 0, MaxWatt);
        /// 可供重量
        public int AvailableWeight => Mathf.Clamp(MaxWeight - UsageWeight, 0, MaxWeight);
        /// 可供U層數
        public int AvailableHeightU => Mathf.Clamp(MaxHeightU - UsageHeightU, 0, MaxHeightU);

        /// 百分比：可供電力 
        public float AvailableWattPercentage01 => (float)AvailableWatt / (MaxWatt);
        /// 百分比：可供重量
        public float AvailableWeightPercentage01 => (float)AvailableWeight / MaxWeight;
        /// 百分比：可供U層數
        public float AvailableHeightUPercentage01 => (float)AvailableHeightU / MaxHeightU;
        
        /// 即時溫度
        public float RT => Random.Range(14f, 30f);

        /// 即時濕度
        public float RH => Random.Range(55f, 85f);

        #endregion

        /// 設備是否適放於至機櫃
        public bool IsDeviceSuitable(UploadDeviceRevitAssetData deviceData)
        {
            bool isWattSuitable = deviceData.Watt < AvailableWatt;
            bool isWeightSuitable = deviceData.Weight < AvailableWeight;
            bool isHeightSuitable = deviceData.HeightU < AvailableHeightU;
            return isWattSuitable && isWeightSuitable && isHeightSuitable;
        }
        
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
            RackNo = DeviceNameAndCode.Split("+")[1].Trim();
        }
    }
}