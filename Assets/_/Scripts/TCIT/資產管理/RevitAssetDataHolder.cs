using _VictorDev.ApiExtensions;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// AssetData資料持有
    [DisallowMultipleComponent]
    public class RevitAssetDataHolder:MonoBehaviour
    {
        [Label("[資料項 - 機櫃]"), SerializeField, ShowIf(nameof(IsRackAsset))] private RackRevitAssetData rackRevitAssetData;
        [Label("[資料項 - 設備]"), SerializeField, ShowIf(nameof(IsDeviceAsset))]private DeviceRevitAssetData deviceRevitAssetData;

        public RackRevitAssetData RackRevitData => rackRevitAssetData;
        public DeviceRevitAssetData DeviceRevitData => deviceRevitAssetData;
        public bool IsRackAsset => string.IsNullOrEmpty(rackRevitAssetData?.DevicePath) == false; 
        public bool IsDeviceAsset => string.IsNullOrEmpty(deviceRevitAssetData?.DevicePath) == false; 
        public EnvironmentData EnvData => envData ??= GetComponent<EnvironmentDataHolder>().EnvData;
        private EnvironmentData envData;
        
        /// 接收RackAssetData
        public void ReceiveAssetData(RevitAssetData revitAssetData)
        {
            switch (revitAssetData.RevitAssetKind)
            {
                case EnumRevitAssetKind.Rack: rackRevitAssetData = revitAssetData as RackRevitAssetData; break;
                default: deviceRevitAssetData = revitAssetData as DeviceRevitAssetData; break;
            }
        }

        [Button, ShowIf(nameof(IsRackAsset))]
        private void GetAvailableUSize() => Debug.Log($"GetAvailableUSize: {rackRevitAssetData.AvailableUSize.ToPrint()}");
    }
}