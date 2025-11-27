using System;
using System.Collections.Generic;
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
        [field: SerializeField] public List<DeviceRevitAssetData> Containers { get; private set; }

        #region 額外增加的變數

        /// 機櫃編號
        public string RackNo => DeviceNameAndCode.Split("+")[1];
        
        public int UsageWatt
        {
            get
            {
                if (usageWatt == 0) usageWatt = (int)Random.Range(Information.watt*0.1f,Information.watt*0.5f);
                return usageWatt;
            }
        }
        public int UsageWeight
        {
            get
            {
                if (usageWeight == 0) usageWeight = (int)Random.Range(Information.weight*0.1f,Information.weight*0.5f);
                return usageWeight;
            }
        }
        private int usageWatt, usageWeight;

        public int MaxWatt => Information.watt;
        public int MaxWeight => Information.weight;
        public int MaxHeightU => Information.heightU;

        /// 即時溫度
        public float RT => Random.Range(14f, 30f);
        /// 即時濕度
        public float RH => Random.Range(55f, 85f);
        #endregion
        
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
        }
    }
}