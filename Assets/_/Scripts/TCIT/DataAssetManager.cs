using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using _VictorDev.Frameworks;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料管理器
    public class DataAssetManager : JsonDataManagerParent<List<RackAssetData>>
    {
        #region Variables
        [Foldout("[模型]"), Label("\tRack"), SerializeField] private List<Transform> rackModels;
        [Foldout("[模型]"), Label("\tServer"), SerializeField] private List<Transform> serverModels;
        [Foldout("[模型]"), Label("\tRouter"), SerializeField] private List<Transform> routerModels;
        [Foldout("[模型]"), Label("\tSwitch"), SerializeField] private List<Transform> switchModels;
        [Foldout("[Event] GetData")] public UnityEvent onGetDataEvent;
        #endregion
        
        [Button]
        public void GetData()
        {
            isLoadingEvent?.Invoke(true);
            onGetDataEvent?.Invoke();
        }

        protected override void BeforeInvokeData() => CombineDataAndModels();

        /// 處理資料集與對應3D模型
        private void CombineDataAndModels()
        {
            Data.ForEach(rack =>
            {
                //機櫃模型
                rack.SetModelFromList(rackModels);
               
               //設備模型
               rack.Containers.ForEach(device =>
               {
                   List<Transform> modelList = device.DeviceKind switch
                   {
                       EnumDeviceKind.Server => serverModels,
                       EnumDeviceKind.Router => routerModels,
                       EnumDeviceKind.Switch => switchModels,
                       _ => null
                   };
                   if (modelList != null) device.SetModelFromList(modelList);
               });
            });
        }

        /// 接收機房模型，並進行分類
        public void ReceiveModels(List<Transform> targets)
        {
            rackModels = ModelFilter("Rack");
            serverModels = ModelFilter("Server");
            routerModels = ModelFilter("Router");
            switchModels = ModelFilter("Switch");
            List<Transform> ModelFilter(string keyWords)
                => targets.Where(target => target.name.Contains(keyWords, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// 將模型移除AssetDataHolder
        [Button]
        private void RemoveAssetDataHolderFromModel()
        {
            AssetDataHolder assetDataHolder;
            Data.ForEach(rack =>
            {
                //機櫃模型
                if (rack.Model.TryGetComponent(out assetDataHolder))
                {
                    ObjectHelper.Destroy(assetDataHolder);
                }
               
                //設備模型
                rack.Containers.ForEach(device =>
                {
                    if (device.Model.TryGetComponent(out assetDataHolder))
                    {
                        ObjectHelper.Destroy(assetDataHolder);
                    }
                });
            });
        }
    }
}