using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料管理器
    public class RevitAssetDataManager : JsonDataManagerParent<List<RackRevitAssetData>>
    {
        #region Variables

        [Foldout("[Event] 在此設定擷取資料的觸發")] public UnityEvent toGetDataEvent;
        [Foldout("[模型]"), Label("\tRack"), SerializeField]
        private List<Transform> rackModels;

        [Foldout("[模型]"), Label("\tServer"), SerializeField]
        private List<Transform> serverModels;

        [Foldout("[模型]"), Label("\tRouter"), SerializeField]
        private List<Transform> routerModels;

        [Foldout("[模型]"), Label("\tSwitch"), SerializeField]
        private List<Transform> switchModels;

        #endregion

        [Button]
        public void ToGetData()
        {
            isLoadingEvent?.Invoke(true);
            toGetDataEvent?.Invoke();
        }

        protected override void BeforeInvokeData() => CombineDataAndModels();

        #region 設定資料與模型
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
        /// 將模型移除AssetDataHolder
        [Button]
        private void RemoveAssetDataHolderFromModel()
        {
            rackModels.ForEach(RemoveRevitAssetDataHolderComponent);
            serverModels.ForEach(RemoveRevitAssetDataHolderComponent);
            routerModels.ForEach(RemoveRevitAssetDataHolderComponent);
            switchModels.ForEach(RemoveRevitAssetDataHolderComponent);

            void RemoveRevitAssetDataHolderComponent(Transform target)
            {
                if (target.TryGetComponent(out RevitAssetDataHolder revitAssetDataHolder))
                {
                    ObjectHelper.Destroy(revitAssetDataHolder);
                }
            }
        }
        #endregion

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
    }
}