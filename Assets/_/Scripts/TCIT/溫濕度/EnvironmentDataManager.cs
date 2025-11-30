using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.DateTimeUtils;
using _VictorDev.DebugUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控資料管理
    public class EnvironmentDataManager : JsonDataManagerParent<List<EnvironmentData>>, IReceiveData<List<RackRevitAssetData>>, ITimer
    {
        #region Variables

        [Label("[資料項] - RackRevitAssetData"), SerializeField] private List<RackRevitAssetData> rackRevitAssetData;
        [Label("[資料持有] - EnvironmentDataHolder"), SerializeField] private List<EnvironmentDataHolder> dataHolders;
        [Foldout("[Event] - 讀取完資料時Invoke")] public UnityEvent onLoadDataCompleteEvent;
        #endregion
        
        /// 暫時性
        public void ReceiveData(List<RackRevitAssetData> data) => rackRevitAssetData = data;

        /// 建立RTRH資料持有者
        [Button]
        private void CreateDataHolder() => rackRevitAssetData.ForEach(rack =>
        {
            EnvironmentDataHolder holder = rack.Model.TryAddComponent<EnvironmentDataHolder>();
            holder.SetRackData(rack);
            dataHolders.Add(holder);
        });
        
        /// 讀取RTRH資料
        [Button]
        public void LoadRtRhData()
        {
            dataHolders.ForEach(holder =>
            {
                holder.SetEnvironmentData(new EnvironmentData()
                {
                    rt = Random.Range(14, 23),
                    rh = Random.Range(50, 68),
                });
            });
            
            onLoadDataCompleteEvent?.Invoke();
        }

        private void Start() => OnTimeFinished();

        public void OnTimeUpdate()=> LoadRtRhData();

        public void OnTimeFinished()
        {
        }
    }
}



