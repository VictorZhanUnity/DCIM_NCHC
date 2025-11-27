using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM.RevitAssetModule
{
    public class ModelDataCombiner : MonoBehaviour, IReceiveData<List<RackRevitAssetData>>, IReceiveData<List<Transform>>
    {
        #region Variables

        [Foldout("模型種類"), SerializeField] private List<Transform> rackModels, serverModels, routerModels, switchModels;
        [Label("Json資料"), SerializeField] private List<RackRevitAssetData> dataList;
        
        #endregion
        
        public void ReceiveData(List<Transform> data)
        {
            rackModels = data.FilterByNameForKeywords(EnumSearchType.Include, "Rack");
            serverModels = data.FilterByNameForKeywords(EnumSearchType.Include, "Server");
            routerModels = data.FilterByNameForKeywords(EnumSearchType.Include, "Router");
            switchModels = data.FilterByNameForKeywords(EnumSearchType.Include, "Switch");
        }

        public void ReceiveData(List<RackRevitAssetData> data)
        {
            dataList = data;
            CombineDataAndModel();
        }

        /// 結合資料與模型
        private void CombineDataAndModel()
        {
            dataList.ForEach(rack =>
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
    }
}
