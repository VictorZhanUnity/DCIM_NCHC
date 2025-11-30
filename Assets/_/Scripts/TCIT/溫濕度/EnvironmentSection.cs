using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環境監控 - 區域劃分
    public class EnvironmentSection : MonoBehaviour
    {
        #region Vairalbes

        [field:SerializeField] public float AverageRt { get; private set; }
        [field:SerializeField] public float AverageRh { get; private set; }
        [Label("[資料項] - 環控資料"), SerializeField] private List<EnvironmentDataHolder> evnDataHolders;
        [Label("[EnvValueDisplay]"), SerializeField] private List<MonoBehaviour> receiverMonoBehaviours;
        [Foldout("[組件]"), SerializeField] private BoxCollider area;

        private List<IEnvDataDisplay> receivers;

        #endregion

        /// 新增EnvValueDisplay組件至Receiver列表
        public void AddEnvDisplayTarget(IEnvDataDisplay target) => receiverMonoBehaviours.Add(target as MonoBehaviour);
        public void ClearEnvDisplayTarget()
        {
            receiverMonoBehaviours.ForEach(target=>ObjectHelper.Destroy(target.gameObject));
            receiverMonoBehaviours.Clear();
            receivers?.Clear();
        }

        /// 尋找Collider範圍裡的Rack模型，以收集EnvDataHolders
        [Button]
        public void GetEnvDataHoldersInArea()
        {
            evnDataHolders = Physics.OverlapBox(area.bounds.center, area.bounds.extents, transform.rotation).ToList()
                .FilterByNameForKeywords(EnumSearchType.Include, "Rack")
                .Select(target => target.GetComponent<EnvironmentDataHolder>())
                .Where(comp=> comp != null).ToList();
        }
        
        /// 計算環控均值
        [Button]
        public void CalculateAverageEnvData()
        {
            AverageRt = evnDataHolders.Average(holder => holder.EnvData.rt);
            AverageRh = evnDataHolders.Average(holder => holder.EnvData.rh);
            Awake();
            receivers?.ForEach(target=>target.UpdateData(this));
        }
        

        [Button]
        private void OnValidate()
        {
            area = GetComponent<BoxCollider>();
            area.isTrigger = true;
            receiverMonoBehaviours = ObjectHelper.CheckTypeOfList<IEnvDataDisplay>(receiverMonoBehaviours);
        }

        private void Awake() => receivers ??= receiverMonoBehaviours.Cast<IEnvDataDisplay>().ToList();
    }

    public interface IEnvDataDisplay
    {
        void UpdateData(EnvironmentSection envSection);
    }
}