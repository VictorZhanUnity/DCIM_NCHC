using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.RevitUtils
{
    /// AssetData資料持有
    [DisallowMultipleComponent]
    public class AssetDataHolder:MonoBehaviour
    {
        [Label("[資料項 - 機櫃]"), SerializeField, ShowIf(nameof(IsRackAsset))] private RackAssetData rackAssetData;
        [Label("[資料項 - 設備]"), SerializeField, ShowIf(nameof(IsDeviceAsset))]private DeviceAssetData deviceAssetData;

        public RackAssetData RackData => rackAssetData;
        public DeviceAssetData DeviceData => deviceAssetData;
        
        public bool IsRackAsset { get; private set; } 
        public bool IsDeviceAsset { get; private set; } 
        
        /// 接收RackAssetData
        public void ReceiveAssetData(AssetDataParent assetData)
        {
            rackAssetData = assetData as RackAssetData;
            IsRackAsset = rackAssetData != null;

            deviceAssetData = assetData as DeviceAssetData;
            IsDeviceAsset = deviceAssetData != null;
        }
    }
}