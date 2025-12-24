using System;
using System.Runtime.Serialization;
using _VictorDev.ApiExtensions;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備資料項, Model存相對應的模型資料
    [Serializable]
    public class UploadDeviceRevitAssetData : RevitAssetData
    {
        [JsonProperty] [field: SerializeField] public string DevicePath { get; protected set; }
        
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
          //  deviceId = DevicePath;
            var index = Random.Range(0, 2);
            Information.watt_limit = (index == 0) ? 350 : 250;
            Information.weight_limit = (index == 0) ? 20 : 10;
            ParseDeviceNameAndCode();
        }

        /// 未來需依照需求而修改
        public override void ParseDeviceNameAndCode()
        {
            RevitAssetKind = DcimHelper.GetDeviceKind(deviceId);
            DeviceKindZh = DcimHelper.GetDeviceKindZh(RevitAssetKind);
            DeviceName = deviceId.Split("+")[2];
            DeviceNameAndCode = deviceId;
        }
    }
}