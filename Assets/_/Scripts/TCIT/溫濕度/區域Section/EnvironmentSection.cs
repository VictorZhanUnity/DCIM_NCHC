using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環境監控 - 區域劃分
    public class EnvironmentSection : MonoBehaviour
    {
        #region Vairalbes

        [HideInInspector] public UnityEvent<bool, EnvironmentSection> onToggleIsOnEvent;

        [field: SerializeField] public float AverageRt { get; private set; }
        [field: SerializeField] public float AverageRh { get; private set; }

        [Label("[資料項] - 環控資料"), SerializeField]
        private List<EnvironmentDataHolder> evnDataHolders;

        [Label("[Value顯示組件] - EnvValueDisplay"), SerializeField]
        private List<EnvironmentValueDisplay> envValueDisplays;

        [Foldout("[組件]"), SerializeField] private BoxCollider area;

        
        public List<Transform> RacksTransformList => racksTransformLis ??= evnDataHolders.Select(x => x.transform).ToList();
        private List<Transform> racksTransformLis;
        
        #endregion

        /// 新增EnvValueDisplay組件至Receiver列表
        public void AddEnvDisplayTarget(EnvironmentValueDisplay target) => envValueDisplays.Add(target);

        public void ClearEnvDisplayTarget()
        {
            envValueDisplays.ForEach(target => ObjectHelper.Destroy(target.gameObject));
            envValueDisplays.Clear();
        }

        /// 尋找Collider範圍裡的Rack模型，以收集EnvDataHolders
        [Button]
        public void GetEnvDataHoldersInArea()
        {
            evnDataHolders = Physics.OverlapBox(area.bounds.center, area.bounds.extents, transform.rotation).ToList()
                .FilterByNameForKeywords(EnumSearchType.Include, "Rack")
                .Select(target => target.GetComponent<EnvironmentDataHolder>())
                .Where(comp => comp != null).ToList();
        }

        /// 計算環控均值
        [Button]
        public void CalculateAverageEnvData()
        {
            AverageRt = evnDataHolders.Average(holder => holder.EnvData.rt);
            AverageRh = evnDataHolders.Average(holder => holder.EnvData.rh);
            envValueDisplays.ForEach(target => target.SetEnvData(this));
        }

        #region Toggle EventListener

        private void OnEnable() =>
            envValueDisplays.ForEach(display => display.ToggleComp.onValueChanged.AddListener(OnToggleValueChanged));

        private void OnDisable() => envValueDisplays.ForEach(display =>
            display.ToggleComp.onValueChanged.RemoveListener(OnToggleValueChanged));

        private void OnToggleValueChanged(bool isOn)
        {
            /*if (isOn)
            {
                envValueDisplays.ForEach(target => target.ToggleComp?.SetIsOnWithoutNotify(true));
            }*/

            onToggleIsOnEvent?.Invoke(isOn, this);
        }

        #endregion

        [Button]
        private void OnValidate()
        {
            area = GetComponent<BoxCollider>();
            area.isTrigger = true;
        }
    }
}