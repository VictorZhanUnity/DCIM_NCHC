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

        [Foldout("[模型]"), Label("\tServer"), SerializeField]
        private List<Transform> serverModels;

        [Foldout("[模型]"), Label("\tRouter"), SerializeField]
        private List<Transform> routerModels;

        [Foldout("[模型]"), Label("\tSwitch"), SerializeField]
        private List<Transform> switchModels;

        #endregion

        protected override void BeforeInvokeData() => CombineDataAndModels();

        #region 設定資料與模型

        /// 處理資料集與對應3D模型
        private void CombineDataAndModels()
        {
            Data.ForEach(uploadDeviceData =>
            {
                List<Transform> modelList = uploadDeviceData.DeviceKind switch
                {
                    EnumDeviceKind.Server => serverModels,
                    EnumDeviceKind.Router => routerModels,
                    EnumDeviceKind.Switch => switchModels,
                    _ => null
                };
                if (modelList != null) uploadDeviceData.SetModelFromList(modelList);
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