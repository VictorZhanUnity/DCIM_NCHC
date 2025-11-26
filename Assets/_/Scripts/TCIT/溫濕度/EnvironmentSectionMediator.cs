using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    public class EnvironmentSectionMediator : MonoBehaviour
    {
        [Label("[資料項 - EnvironmentSection]"), SerializeField] private List<EnvironmentSection> environmentSections;

        /// 取得即時溫濕度
        public void GetRealtimeValue() => environmentSections.ForEach(item => item.GetAverageRtRhFromRacks());
        
        [Button]
        private void GetCompsInChildren() =>
            environmentSections = transform.GetComponentsInChildren<EnvironmentSection>().ToList();
        
        /// 建立Landmark
        [Button]
        public void CreateLandmark() => environmentSections.ForEach(item => item.CreateLandmark());
    }
}

