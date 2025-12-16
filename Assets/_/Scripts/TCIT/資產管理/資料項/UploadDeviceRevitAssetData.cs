using System;
using System.Runtime.Serialization;
using _VictorDev.ApiExtensions;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備資料項, Model存相對應的模型資料
    [Serializable]
    public class UploadDeviceRevitAssetData : RevitAssetData
    {
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context) => ParseDeviceNameAndCode();

        /// 未來需依照需求而修改
        public override void ParseDeviceNameAndCode()
        {
            RevitAssetKind = DcimHelper.GetDeviceKind(DevicePath);
            DeviceKindZh = DcimHelper.GetDeviceKindZh(RevitAssetKind);
            DeviceName = DevicePath.Split("+")[2];
            DeviceNameAndCode = DevicePath;
            
            bool isHaveValue = !string.IsNullOrEmpty(Information.type_manufacturer);
            Manufacturer = isHaveValue ? Information.type_manufacturer : DevicePath.Split("+")[6].Split("-")[0];
        }
    }
}