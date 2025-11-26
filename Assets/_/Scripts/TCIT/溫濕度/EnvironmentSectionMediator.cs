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
        [Foldout("[Event] - 發送溫度平均值"), SerializeField] private UnityEvent<float> invokeAverageRtEvent; 
        [Foldout("[Event] - 發送濕度平均值"), SerializeField] private UnityEvent<float> invokeAverageRhEvent; 
        #endregion

        /// 取得即時溫濕度
        public void GetRealtimeValue()
        {
            float allAverageRT = 0, allAverageRH = 0;
            environmentSections.ForEach(item =>
            {
                allAverageRT += item.AverageRt;
                allAverageRH += item.AverageRh;
                item.GetAverageRtRhFromRacks();
            });
            allAverageRT /= environmentSections.Count;
            allAverageRH /= environmentSections.Count;
            
            invokeAverageRtEvent?.Invoke(allAverageRT);
            invokeAverageRhEvent?.Invoke(allAverageRH);
        }

        [Button]
        private void GetCompsInChildren() =>
            environmentSections = transform.GetComponentsInChildren<EnvironmentSection>().ToList();
        
        /// 建立Landmark
        [Button]
        public void CreateLandmark() => environmentSections.ForEach(item => item.CreateLandmark());
    }
}

