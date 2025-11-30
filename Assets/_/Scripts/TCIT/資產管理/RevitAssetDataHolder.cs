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
        
        public bool IsRackAsset { get; private set; } 
        public bool IsDeviceAsset { get; private set; } 

        public EnvironmentData EnvData => envData ??= GetComponent<EnvironmentDataHolder>().EnvData;
        private EnvironmentData envData;
        
        /// 接收RackAssetData
        public void ReceiveAssetData(RevitAssetData revitAssetData)
        {
            rackRevitAssetData = revitAssetData as RackRevitAssetData;
            IsRackAsset = rackRevitAssetData != null;

            deviceRevitAssetData = revitAssetData as DeviceRevitAssetData;
            IsDeviceAsset = deviceRevitAssetData != null;
        }
    }
}