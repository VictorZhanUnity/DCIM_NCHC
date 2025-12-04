using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DateTimeUtils;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控區域劃分管理器
    public class EnvironmentSectionManager : MonoBehaviour, ITimerUpdate
    {
        #region Variables


        [Label("[環控ValueDisplay設定]"), SerializeField]
        private List<EnvironmentValueDisplaySetting> envDisplaySettings;

        [Label("[資料項 - EnvironmentSection]"), SerializeField] private List<EnvironmentSection> environmentSections;
        
        
        [Foldout("[Event] - 點擊Section時")] public UnityEvent<Transform> invokeBodyOnClickSectionEvent;
        [Foldout("[Event] - 點擊Section時")] public UnityEvent<EnvironmentSection> invokeSectionOnClickSectionEvent;
        [Foldout("[Event] - 所有Section都未選取時Invoke")] public UnityEvent onNonSelectedSectionEvent; 
        
        [Foldout("[Event] - 發送所有溫度平均值")] public UnityEvent<float> invokeAverageRtEvent; 
        [Foldout("[Event] - 發送所有濕度平均值")] public UnityEvent<float> invokeAverageRhEvent; 
        [Foldout("[組件]"), SerializeField] private ToggleGroup toggleGroup; 
        
        #endregion

        /// 各個Section取得範圍裡的EnvValueDataHolder
        [Button]
        private void GetEnvDataHoldersInAreaBySections() => environmentSections.ForEach(section => section.GetEnvDataHoldersInArea());

        /// 依設定建立EnvValueDisplay組件
        [Button]
        private void CreateEnvValueDisplaysBySettings()
        {
            environmentSections.ForEach(section =>
            {
                section.ClearEnvDisplayTarget();
                envDisplaySettings.ForEach(setting =>
                {
                    EnvironmentValueDisplay envValueDisplay = ObjectHelper.Instantiate(setting.envDisplayPrefab, setting.container);
                    envValueDisplay.name += $" - {section.name}";
                    envValueDisplay.SetToggleGroup(toggleGroup);
                    envValueDisplay.SetEnumEnvDataType(setting.envDataType);
                    section.AddEnvDisplayTarget(envValueDisplay);
                });
                section.CalculateAverageEnvData();
            });
        }

        /// 清除所有EnvValueDisplay組件
        [Button]
        private void DeleteAllEnvValueDisplays() => environmentSections.ForEach(section => section.ClearEnvDisplayTarget());

        /// 計算環控均值
        public void CalculateAverageValue()
        {
            float allAverageRt = 0, allAverageRh = 0;
            environmentSections.ForEach(section =>
            {
                section.CalculateAverageEnvData();
                allAverageRt += section.AverageRt;
                allAverageRh += section.AverageRh;
            });
            allAverageRt /= environmentSections.Count;
            allAverageRh /= environmentSections.Count;
            
            invokeAverageRtEvent?.Invoke(allAverageRt);
            invokeAverageRhEvent?.Invoke(allAverageRh);
        }
        
        public void OnTimeUpdate() => CalculateAverageValue();

        #region Toggle EventListener
        
        private void Start()
        {
            environmentSections.ForEach(section => section.onToggleIsOnEvent.AddListener(OnToggleIsOnEventHandler));
        }

        private void OnToggleIsOnEventHandler(bool isOn, EnvironmentSection section)
        {
            if (isOn)
            {
                invokeBodyOnClickSectionEvent?.Invoke(section.transform);
                invokeSectionOnClickSectionEvent?.Invoke(section);
            }
            else
                onNonSelectedSectionEvent?.Invoke();
        }

        #endregion
        
        [Button]
        private void OnValidate()
        {
            if(environmentSections.Count == 0) environmentSections = FindObjectsByType<EnvironmentSection>(FindObjectsSortMode.None).ToList();
            toggleGroup = transform.TryAddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = true;
        }
       
        /// 環控Landmark設定
        [Serializable]
        public class EnvironmentValueDisplaySetting
        {
            public EnumEnvDataType envDataType;
            public Transform container;
            public EnvironmentValueDisplay envDisplayPrefab;
        }
    }
    public enum EnumEnvDataType
    {
        None, RT, RH, 
    }
}

