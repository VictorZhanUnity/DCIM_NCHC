using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料
    [Serializable]
    public class DeviceAssetData : AssetDataParent
    {
        #region Struct Variables

        [JsonProperty] [field: SerializeField] public string RackDevicePath { get; private set; }
        [JsonProperty] [field: SerializeField] public int RackLocation { get; private set; }

        #endregion

        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
        }
    }
}