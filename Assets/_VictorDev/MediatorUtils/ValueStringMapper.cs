using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using UnityEngine;
using UnityEngine.Events;

public class ValueStringMapper : MonoBehaviour
{
    public UnityEvent<string> toMapValueStringEvent;
    
    public List<KeyValueData<float, string>> mapperSetting;
    
    public void SetValue(float value)
    {
        var result = mapperSetting.FirstOrDefault(setting => setting.Key.ToString() == value.ToString());
        toMapValueStringEvent?.Invoke( result != null ? result.Value.ToString() : value.ToString());
    }
}
