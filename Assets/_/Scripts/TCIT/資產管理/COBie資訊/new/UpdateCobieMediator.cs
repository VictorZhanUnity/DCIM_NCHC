using System;
using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using UnityEngine;
using UnityEngine.Events;

public class UpdateCobieMediator : MonoBehaviour
{

    private readonly List<KeyValueData<string, string>> updateCobieInfo = new List<KeyValueData<string, string>>()
    {
        new KeyValueData<string, string>("DeviceCode", ""),
        new KeyValueData<string, string>("Key", ""),
        new KeyValueData<string, string>("Value", ""),
    };

    public UnityEvent<List<KeyValueData<string, string>>> invokeUpdateCobieColumnInfo;
    
    public void SetCobieColumnInfo(string deviceCode, string columnName, string value)
    {
        updateCobieInfo.ForEach(item =>
        {
            if (item.Key.Equals("DeviceCode", StringComparison.OrdinalIgnoreCase)) item.Value = deviceCode;
            else if (item.Key.Equals("Key", StringComparison.OrdinalIgnoreCase)) item.Value = columnName;
            else if (item.Key.Equals("Value", StringComparison.OrdinalIgnoreCase)) item.Value = value;
        });
        invokeUpdateCobieColumnInfo?.Invoke(updateCobieInfo);
    }
}
