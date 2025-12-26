using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DateTimeUtils;
using _VictorDev.MediatorUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控資料管理
    public class EnvironmentDataManager : JsonDataManagerParent<List<EnvironmentData>>
    {
        #region Variables

        [Label("[資料持有] - EnvironmentDataHolder"), SerializeField] private List<EnvironmentDataHolder> environmentDataHolders;
        [Foldout("[Event] - 讀取完資料時Invoke")] public UnityEvent onLoadDataCompleteEvent;
        [Foldout("[組件]"), SerializeField] private RevitAssetDataManager revitAssetDataManager;
        
        public List<EnvironmentDataHolder> EnvironmentDataHolders => environmentDataHolders;
        #endregion

        /// 暫時性
        protected override void BeforeInvokeData()
        {
            Data.ForEach(data=>
            {
                var result =environmentDataHolders.FirstOrDefault(dataHolder => dataHolder.EnvData.deviceCode.Equals(data.deviceCode));
                if(result != null) result.SetEnvironmentData(data);
            });

            ShowHeatColor_RT();
            onLoadDataCompleteEvent?.Invoke();
        }

        /// 建立RTRH資料持有者
        [Button]
        private void CreateDataHolder()
        {
            environmentDataHolders.Clear();
            revitAssetDataManager.Data.ForEach(rack =>
            {
                EnvironmentDataHolder holder = rack.Model.TryAddComponent<EnvironmentDataHolder>();
                holder.UpdateDeviceCode();
                environmentDataHolders.Add(holder);
            });
        }
        
        /*/// 讀取RTRH資料
        [Button]
        public void LoadRtRhData()
        {
            environmentDataHolders.ForEach(holder =>
            {
                holder.SetEnvironmentData(new EnvironmentData()
                {
                    RTValue = DcimSysConfig.RTValueValueRangeDEMO.GetRandomValue(),
                    RHValue = (int)DcimSysConfig.RhValueValueRangeDEMO.GetRandomValue(),
                });
            });
            
            onLoadDataCompleteEvent?.Invoke();
        }

        public void OnTimeUpdate()=> LoadRtRhData();*/

        public void ShowHeatColor_RT() => environmentDataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.RT));
        public void ShowHeatColor_RH() => environmentDataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.RH));
        public void ShowSourceRackColor() => environmentDataHolders.ForEach(holder=> holder.SetRackDisplayType(EnumEnvDataType.None));
    }
}



