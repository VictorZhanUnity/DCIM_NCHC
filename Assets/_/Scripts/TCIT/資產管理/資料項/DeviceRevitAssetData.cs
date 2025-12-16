using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料
    [Serializable]
    public class DeviceRevitAssetData : RevitAssetData
    {
        #region Struct Variables

        [JsonProperty] [field: SerializeField] public string RackDevicePath { get; private set; }
        [JsonProperty] [field: SerializeField] public int RackLocation;

        #endregion

        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
            
            //Revit冠宇工具的RackLocation計算錯誤，忽略了櫃體底部滑輪的高度，大約3U
            RackLocation = (RackLocation == 0) ? 42 : RackLocation - 2;
        }
    }
}