using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VictorDev.TCIT;

namespace _VictorDev.TCIT
{
    /// 判斷點擊模型的類型，做相對應的資料Invoke
    public class ObjectSelectionMediator : MonoBehaviour
    {
        #region Variables
        [Foldout("[Event] 點擊模型 - 機櫃")] public UnityEvent<RackAssetData> onRackClickedEvent;
        [Foldout("[Event] 點擊模型 - 設備")] public UnityEvent<DeviceAssetData> onDeviceClickedEvent;
        #endregion
        
        /// 接收目前點擊的模型
        public void ReceiveOnClickedModel(Transform model)
        {
            IsRackOrDeviceModel(model);
        }

        private bool IsRackOrDeviceModel(Transform model)
        {
            if (model.TryGetComponent(out AssetDataHolder assetDataHolder))
            {
                if(assetDataHolder.IsRackAsset) onRackClickedEvent?.Invoke(assetDataHolder.RackData);
                else onDeviceClickedEvent?.Invoke(assetDataHolder.DeviceData);
                return true;
            }
            return false;
        }
    }
}
