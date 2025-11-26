using System;
using _VictorDev.MediatorUtils;
using _VictorDev.Framework;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule.Old
{
    /// 環境控制 Landmark，顯示任一項目數值用 (溫度 or 濕度... etc)
    public class EnvironmentLandmark : MonoBehaviour
    {
        #region Vairalbes

        [Foldout("[組件]"), SerializeField] private ValueMediator txtValue;
        [Foldout("[組件]"), SerializeField] private PositionTo2DPoint positionTo2DPoint;
        [Foldout("[組件]"), SerializeField] private Toggle toggle;

        public Toggle ToggleComp => toggle;
        
        #endregion

        public void SetToggleGroup(ToggleGroup group) => toggle.group = group;
        
        /// 設定定位目標
        public void SetTargetModel(Transform target) => positionTo2DPoint.SetTargetObject(target);
        /// 設定值
        public void SetValue(float value) => txtValue.SetValue(value);

        [Button]
        public void FindComponents()
        {
            txtValue = GetComponentInChildren<ValueMediator>(true);
            positionTo2DPoint = GetComponent<PositionTo2DPoint>();
            positionTo2DPoint.FindComponents();
            toggle = GetComponent<Toggle>();
        }
        
        private void Reset() => FindComponents();
    }
}