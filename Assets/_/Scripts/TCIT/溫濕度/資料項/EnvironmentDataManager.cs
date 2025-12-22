using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.DateTimeUtils;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控資料管理
    public class EnvironmentDataManager : JsonDataManagerParent<List<EnvironmentData>>, ITimerUpdate
    {
        #region Variables

        [Label("[資料持有] - EnvironmentDataHolder"), SerializeField] private List<EnvironmentDataHolder> dataHolders;
        [Foldout("[Event] - 讀取完資料時Invoke")] public UnityEvent onLoadDataCompleteEvent;
        [Foldout("[組件]"), SerializeField] private RevitAssetDataManager revitAssetDataManager;
        
        public List<EnvironmentDataHolder> DataHolders => dataHolders;
        #endregion
        
        /// 暫時性

        /// 建立RTRH資料持有者
        [Button]
        private void CreateDataHolder()
        {
            dataHolders.Clear();
            revitAssetDataManager.Data.ForEach(rack =>
            {
                EnvironmentDataHolder holder = rack.Model.TryAddComponent<EnvironmentDataHolder>();
                dataHolders.Add(holder);
            });
        }

        
        /// 產生RTRH假資料
        [Button]
        public void GenerateRtRhData()
        {
            dataHolders.ForEach(holder =>
            {
                holder.SetEnvironmentData(new EnvironmentData()
                {
                    RTValue = DcimSysConfig.RTValueValueRangeDEMO.GetRandomValue(),
                    RHValue = (int)DcimSysConfig.RhValueValueRangeDEMO.GetRandomValue(),
                });
            });
            
            onLoadDataCompleteEvent?.Invoke();
        }

        public void OnTimeUpdate()=> GenerateRtRhData();

        /// 設定顯示溫度
        public void ShowHeatColor_RT() => dataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.RT));
        /// 設定顯示濕度
        public void ShowHeatColor_RH() => dataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.RH));
        /// 顯示機櫃原始材質
        public void ShowSourceRackColor() => dataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.None));
    }
}



