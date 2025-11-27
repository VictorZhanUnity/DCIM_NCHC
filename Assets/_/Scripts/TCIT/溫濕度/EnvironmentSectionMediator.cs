using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    public class EnvironmentSectionMediator : MonoBehaviour
    {
        #region Variables
        [Label("[資料項 - EnvironmentSection]"), SerializeField] private List<EnvironmentSection> environmentSections;
        
        [Foldout("[Event] - 發送所有溫度平均值")] public UnityEvent<float> invokeAverageRtEvent; 
        [Foldout("[Event] - 發送所有濕度平均值")] public UnityEvent<float> invokeAverageRhEvent; 
        [Foldout("[Event] - 點擊Section時"), Label("Invoke Section本身")] public UnityEvent<Transform> invokeClickSectionBodyEvent;
        [Foldout("[Event] - 點擊Section時"), Label("Invoke Section機櫃群")] public UnityEvent<List<Transform>> invokeClickSectionRacksEvent;
        [Foldout("[Event] - 點擊Section時"), Label("Invoke Section機櫃群")] public UnityEvent<EnvironmentSection> invokeClickSectionEvent;
        [Foldout("[Event] - 所有Section都未選取時Invoke")] public UnityEvent invokeOnCancelAllSectionEvent; 
        #endregion

        /// 取得即時溫濕度
        public void GetRealtimeValue()
        {
            float allAverageRt = 0, allAverageRh = 0;
            environmentSections.ForEach(item =>
            {
                allAverageRt += item.AverageRt;
                allAverageRh += item.AverageRh;
                item.GetAverageRtRhFromRacks();
            });
            allAverageRt /= environmentSections.Count;
            allAverageRh /= environmentSections.Count;
            
            invokeAverageRtEvent?.Invoke(allAverageRt);
            invokeAverageRhEvent?.Invoke(allAverageRh);
        }

        [Button]
        private void GetEnvironmentSectionsInChildren() =>
            environmentSections = transform.GetComponentsInChildren<EnvironmentSection>().ToList();
        
        /// 建立Landmark
        [Button]
        public void CreateLandmark() => environmentSections.ForEach(item => item.CreateLandmark());


        #region EventListener
        private void OnEnable() => environmentSections.ForEach(section =>
        {
            section.onClickSectionEvent.AddListener(OnClickSection);
            section.onCancelSectionEvent.AddListener(OnCancelSection);
        });

        private void OnDisable() => environmentSections.ForEach(section =>
        {
            section.onClickSectionEvent.RemoveListener(OnClickSection);
            section.onCancelSectionEvent.RemoveListener(OnCancelSection);
        });
        private void OnClickSection(EnvironmentSection environmentSection)
        {
            invokeClickSectionBodyEvent?.Invoke(environmentSection.transform);
            invokeClickSectionRacksEvent?.Invoke(environmentSection.RackModels);
            invokeClickSectionEvent?.Invoke(environmentSection);
        }

        private void OnCancelSection()
        {
            bool isAllCancel = environmentSections.All(section=> section.IsSelected == false);
            if (isAllCancel) invokeOnCancelAllSectionEvent?.Invoke();
        }
        #endregion
       
    }
}

