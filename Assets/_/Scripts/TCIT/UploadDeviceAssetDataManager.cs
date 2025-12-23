using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備資料管理器
    public class UploadDeviceAssetDataManager : JsonDataManagerParent<List<UploadDeviceRevitAssetData>>
    {
        #region Variables

        [Foldout("[Event] 在此設定擷取資料的觸發")] public UnityEvent toGetDataEvent;

        [Foldout("[設備模型Prefab]"), Label("\tServer"), SerializeField]
        private List<Transform> serverModels;

        [Foldout("[設備模型Prefab]"), Label("\tRouter"), SerializeField]
        private List<Transform> routerModels;

        [Foldout("[設備模型Prefab]"), Label("\tSwitch"), SerializeField]
        private List<Transform> switchModels;

        #endregion

        protected override void BeforeInvokeData() => CombineDataAndModels();

        #region 設定資料與模型

        /// 處理資料集與對應3D模型
        private void CombineDataAndModels()
        {
            Data.ForEach(uploadDeviceData =>
            {
                List<Transform> modelList = uploadDeviceData.RevitAssetKind switch
                {
                    EnumRevitAssetKind.Server => serverModels,
                    EnumRevitAssetKind.Router => routerModels,
                    EnumRevitAssetKind.Switch => switchModels,
                    _ => null
                };
                if (modelList != null) uploadDeviceData.SetModelAndHolderFromList(modelList);
            });
        }

        #endregion


        [Button]
        public void ToGetData()
        {
            isLoadingEvent?.Invoke(true);
            toGetDataEvent?.Invoke();
        }

        /// 接收機房模型，並進行分類
        public void ReceiveModels(List<Transform> targets)
        {
            serverModels = targets.FilterByNameForKeywords(EnumSearchType.Include, "Server");
            routerModels = targets.FilterByNameForKeywords(EnumSearchType.Include, "Router");
            switchModels = targets.FilterByNameForKeywords(EnumSearchType.Include, "Switch");
        }
    }
}