using System;
using _VictorDev.CameraUtils;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 判斷點擊模型的類型，做相對應的資料Invoke
    public class ObjectSelectionMediator : MonoBehaviour
    {
        #region Variables
        [Foldout("[Event] 點擊模型 - 機櫃")] public UnityEvent<RackRevitAssetData> onRackClickedEvent;
        [Foldout("[Event] 點擊模型 - 設備")] public UnityEvent<DeviceRevitAssetData> onDeviceClickedEvent;
        [Foldout("[設定]"), SerializeField] private float camDistanceToRack=4f, camDistanceToDevice = 1.5f;
        private Transform targetModel;
        
        #endregion
        
        /// 接收目前點擊的模型
        public void ReceiveOnClickedModel(Transform model)
        {
            targetModel = model;
            IsRackOrDeviceModel(targetModel);
        }

        private bool IsRackOrDeviceModel(Transform model)
        {
            if (model.TryGetComponent(out RevitAssetDataHolder assetDataHolder))
            {
                if (assetDataHolder.IsRackAsset)
                {
                    onRackClickedEvent?.Invoke(assetDataHolder.RackRevitData);
                }
                else
                {
                    onDeviceClickedEvent?.Invoke(assetDataHolder.DeviceRevitData);
                }
                RTSCameraController.CameraToPosition(targetModel, assetDataHolder.IsRackAsset? camDistanceToRack: camDistanceToDevice);
                return true;
            }
            return false;
        }
    }
}
