using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DebugUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.Framework;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環境控制 Landmark，顯示任一項目數值用 (溫度 or 濕度... etc)
    public class EnvironmentValueDisplay : MonoBehaviour
    {
        #region Vairalbes

        [SerializeField] private EnumEnvDataType envDataType;
        [Foldout("[組件]"), SerializeField] private ValueMediator valueMediator;
        [Foldout("[組件]"), SerializeField] private Toggle toggle;
        [Foldout("[組件]"), SerializeField] private TextDotweener txtTitle;
        [Foldout("[組件]"), SerializeField] private PositionTo2DPoint positionTo2DPoint;

        public Toggle ToggleComp => toggle;

        #endregion
        
        public void SetEnumEnvDataType(EnumEnvDataType value) => envDataType = value;

        public void SetEnvData(EnvironmentSection envSection)
        {
            txtTitle?.SetText(envSection.name);
            positionTo2DPoint?.SetTargetObject(envSection.transform);
            float result = envDataType switch
            {
                EnumEnvDataType.RT => envSection.AverageRt,
                EnumEnvDataType.RH => envSection.AverageRh,
                _ => 0
            };
            valueMediator.SetValue(result);
        }

        public void SetToggleGroup(ToggleGroup group) => toggle.group = group;

        [Button]
        private void OnValidate()
        {
            valueMediator = GetComponentInChildren<ValueMediator>(true);
            positionTo2DPoint ??= GetComponent<PositionTo2DPoint>();
            positionTo2DPoint?.OnValidate();
            toggle ??= GetComponent<Toggle>();
        }
    }
}