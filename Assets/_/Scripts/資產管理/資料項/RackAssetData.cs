using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace _VictorDev.RevitUtils
{
    /// 機櫃資料
    [Serializable]
    public class RackAssetData : AssetDataParent
    {
        /// 設備列表
        [JsonProperty]
        [field: SerializeField] public List<DeviceAssetData> Containers { get; private set; }

        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
        }
    }
}