using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule.Old;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環境監控 - 區域管理
    public class EnvironmentSection : MonoBehaviour
    {
        #region Vairalbes
        [Label("[機櫃群 - 資料]"), SerializeField] private List<RevitAssetDataHolder> dataHolders;
        [Label("[項目設定]"), SerializeField] private List<EnvironmentItem> environmentItems;
        /// 點選Section時Invoke Section本身與所屬機櫃群
        [HideInInspector] public UnityEvent<EnvironmentSection> onClickSectionEvent;
        [HideInInspector] public UnityEvent onCancelSectionEvent;
        [Foldout("[組件]"), SerializeField] private BoxCollider area;
        [Foldout("[組件]"), SerializeField] private ToggleGroup toggleGroup;

        public float AverageRt { get; private set; }
        public float AverageRh { get; private set; }

        public bool IsSelected => environmentItems.Any(item => item.ToggleComp.isOn);

        ///機櫃群模型
        public List<Transform> RackModels { get; private set; }

        #endregion


        [ContextMenu("CreateLandmark")]
        public void CreateLandmark()
        {
            environmentItems.ForEach(item =>
            {
               EnvironmentLandmark landmark =  item.Instantiate(gameObject.name);
               landmark.SetToggleGroup(toggleGroup);
               landmark.SetTargetModel(transform);
               landmark.FindComponents();
            });
            GetAverageRtRhFromRacks();
        }

        [ContextMenu("GetAverageRtRhFromRacks")]
        public void GetAverageRtRhFromRacks()
        {
            AverageRt = dataHolders.Average(holder => holder.RackRevitData.RT);
            AverageRh = dataHolders.Average(holder => holder.RackRevitData.RH);
            environmentItems[0].landmark.SetValue(AverageRt);
            environmentItems[1].landmark.SetValue(AverageRh);
        }
        
        /// 尋找Collider範圍裡的Rack模型
        [ContextMenu("FindRacksInArea")]
        private void FindRacksInArea()
        {
            dataHolders = Physics.OverlapBox(area.bounds.center, area.bounds.extents, transform.rotation).ToList()
                .FilterByNameForKeywords(EnumSearchType.Include, "Rack")
                .FilterByNameForKeywords(EnumSearchType.Exclude, "RackUnitGrid")
                .Select(target => target.GetComponent<RevitAssetDataHolder>()).ToList();
        }

        [ContextMenu("FindComponents")]
        private void FindComponents()
        {
            area = GetComponent<BoxCollider>();
            toggleGroup = GetComponentInParent<ToggleGroup>(true);
        }

        private void Reset() => FindComponents();

        #region EventListener

        private void OnEnable()
        {
            GetAverageRtRhFromRacks();
            environmentItems.ForEach(item=> item.ToggleComp.onValueChanged.AddListener(OnToggleClicked));
        }

        private void OnDisable() => environmentItems.ForEach(item=> item.ToggleComp.onValueChanged.RemoveListener(OnToggleClicked));
        private void OnToggleClicked(bool isOn)
        {
            RackModels ??= dataHolders.Select(data=> data.transform).ToList();
            if(isOn) onClickSectionEvent?.Invoke(this);
            else onCancelSectionEvent?.Invoke();
        } 

        #endregion
    }

    /// 環境監控項目 - 溫度 / 濕度
    [Serializable]
    public class EnvironmentItem
    {
        public Transform container;
        public EnvironmentLandmark landmark;
        public EnvironmentLandmark landmarkPrefab;

        public Toggle ToggleComp => landmark.ToggleComp;
        
        public EnvironmentLandmark Instantiate(string sectionName)
        {
            if (landmark != null && container.Contain(landmark)) return landmark;
            landmark = ObjectHelper.Instantiate(landmarkPrefab, container);
            landmark.name += $" - {sectionName}";
            return landmark;
        }
    }
  
}