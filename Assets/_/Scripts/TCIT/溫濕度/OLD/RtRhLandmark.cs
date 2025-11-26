using System;
using System.Linq;
using _VictorDev.MediatorUtils;
using _VictorDev.Framework;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule.Old.Old
{
    public class RtRhLandmark : MonoBehaviour
    {
        #region Vairalbes

        [Foldout("[組件]"), SerializeField] private ValueMediator txtRT, txtRH;
        [Foldout("[組件]"), SerializeField] private PositionTo2DPoint positionTo2DPoint;

        #endregion

        public void SetTargetModel(Transform target) => positionTo2DPoint.SetTargetObject(target);
        public void SetRtValue(float value) => txtRT.SetValue(value);
        public void SetRhValue(float value) => txtRH.SetValue(value);
        
        [Button]
        public void FindComponents()
        {
            var result = GetComponentsInChildren<ValueMediator>(true);
            txtRT = result.FirstOrDefault(target => target.name.Equals("TxtRT", StringComparison.OrdinalIgnoreCase));
            txtRH = result.FirstOrDefault(target => target.name.Equals("TxtRH", StringComparison.OrdinalIgnoreCase));
            positionTo2DPoint = GetComponent<PositionTo2DPoint>();
        }

        private void Reset() => FindComponents();
    }
}