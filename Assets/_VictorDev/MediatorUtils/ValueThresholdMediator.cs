using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.MediatorUtils
{
    public class ValueThresholdMediator : MonoBehaviour
    {
       public List<ValueEventMappingSetting> valueEventMappingSettings;

       public void SetValue(float value)
       {
           valueEventMappingSettings.ForEach(setting =>
           {
               switch (setting.compare)
               {
                   case EnumValueCompare.Bigger:
                       if (value > setting.threshold) setting.onConditionEvent?.Invoke();
                       break;
                   case EnumValueCompare.Equal:
                       if (Mathf.Approximately(value, setting.threshold)) setting.onConditionEvent?.Invoke();
                       break;
                   case EnumValueCompare.Lesser:
                       if (value < setting.threshold) setting.onConditionEvent?.Invoke();
                       break;
               }
           });
       }
    }

    [Serializable]
    public class ValueEventMappingSetting
    {
        public EnumValueCompare compare;
        public float threshold;
        public UnityEvent onConditionEvent;
    }

    public enum EnumValueCompare
    {
        Bigger, Lesser, Equal
    }
}