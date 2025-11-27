using _VictorDev.CameraUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 判斷點擊模型的類型，做相對應的資料Invoke
    public class ObjectSelectionMediator : MonoBehaviour
    {
        #region Variables
        [Foldout("[Event] 點擊模型 - 機櫃")] public UnityEvent<RackRevitAssetData> onRackClickedDataEvent;
        [Foldout("[Event] 點擊模型 - 機櫃")] public UnityEvent<Transform> onRackClickedModelEvent;
        [Foldout("[Event] 點擊模型 - 設備")] public UnityEvent<DeviceRevitAssetData> onDeviceClickedDataEvent;
        [Foldout("[Event] 點擊模型 - 設備")] public UnityEvent<Transform> onDeviceClickedRackModelEvent;
        [Foldout("[Event] 取消選取模型時")] public UnityEvent unSelectObjectEvent;
        [Foldout("[設定]"), SerializeField] private float camDistanceToRack=4f, camDistanceToDevice = 1.5f;
        private Transform targetModel;
        
        #endregion
        
        /// 接收目前點擊的模型
        public void ReceiveOnClickedModel(Transform model)
        {
            targetModel = model;
            IsRackOrDeviceModel(targetModel);
        }

        public void UnSelectObject()
        {
            unSelectObjectEvent?.Invoke();
        }

        private bool IsRackOrDeviceModel(Transform model)
        {
            if (model.TryGetComponent(out RevitAssetDataHolder assetDataHolder))
            {
                if (assetDataHolder.IsRackAsset)
                {
                    onRackClickedDataEvent?.Invoke(assetDataHolder.RackRevitData);
                    onRackClickedModelEvent?.Invoke(assetDataHolder.RackRevitData.Model);
                }
                else
                {
                    onDeviceClickedDataEvent?.Invoke(assetDataHolder.DeviceRevitData);
                    onDeviceClickedRackModelEvent?.Invoke(assetDataHolder.DeviceRevitData.Model.parent); //Invoke機櫃模型，以顯示全機櫃
                }
                RTSCameraController.CameraToPosition(targetModel, assetDataHolder.IsRackAsset? camDistanceToRack: camDistanceToDevice);
                return true;
            }
            return false;
        }
    }
}
