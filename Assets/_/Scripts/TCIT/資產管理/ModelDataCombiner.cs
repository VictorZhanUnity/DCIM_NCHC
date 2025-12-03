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
    public class ModelDataCombiner : MonoBehaviour, IReceiveData<List<Transform>>
    {
        #region Variables

        [Foldout("模型種類"), SerializeField] private List<Transform> rackModels, serverModels, routerModels, switchModels
            , odfModels, rackStationModels;
        
        [SerializeField] private RevitAssetDataManager revitAssetDataManager;
        
        #endregion
        
        public void ReceiveData(List<Transform> data)
        {
            rackModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.Rack.ToString());
            serverModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.Server.ToString());
            routerModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.Router.ToString());
            switchModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.Switch.ToString());
            odfModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.ODF.ToString());
            rackStationModels = data.FilterByNameForKeywords(EnumSearchType.Include, EnumRevitAssetKind.RackStation.ToString());
        }
      
        [Button]
        /// 結合資料與模型
        private void CombineDataAndModel()
        {
            revitAssetDataManager.Data.ForEach(rack =>
            {
                //機櫃模型
                rack.SetModelFromList(rackModels);

                //設備模型
                rack.Containers.ForEach(device =>
                {
                    List<Transform> modelList = device.RevitAssetKind switch
                    {
                        EnumRevitAssetKind.Server => serverModels,
                        EnumRevitAssetKind.Router => routerModels,
                        EnumRevitAssetKind.Switch => switchModels,
                        EnumRevitAssetKind.ODF => odfModels,
                        EnumRevitAssetKind.RackStation => rackStationModels,
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
            odfModels.ForEach(RemoveRevitAssetDataHolderComponent);
            rackStationModels.ForEach(RemoveRevitAssetDataHolderComponent);

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
