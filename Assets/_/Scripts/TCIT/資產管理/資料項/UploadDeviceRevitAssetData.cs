using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備資料項, Model存相對應的模型資料
    [Serializable]
    public class UploadDeviceRevitAssetData : RevitAssetData
    {
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
        }

        protected override void ParseDeviceNameAndCode()
        {
            RevitAssetKind = DcimHelper.GetDeviceKind(DevicePath);
            DeviceKindZh = DcimHelper.GetDeviceKindZh(RevitAssetKind);
            DeviceName = DevicePath.Split("+")[2];
            DeviceNameAndCode = "";
        }
    }
}