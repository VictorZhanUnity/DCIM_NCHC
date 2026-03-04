using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;

/// [暫用] - 設備Type 電力、重量、HieightU對應表
public class DeviceUseageMapping : MonoBehaviour
{
    [SerializeField] private UploadDeviceAssetDataManager uploadDeviceAssetDataManager;
    [Foldout("對照表 - [Type, Usage資訊]"), SerializeField] private List<KeyValueData<string, DeviceMappingItem>> deviceMappingTable;
    
    public void MappingInformation()
    {
        uploadDeviceAssetDataManager.Data.ForEach(deviceData =>
        {
            if (deviceData.Watt == 0 || deviceData.Watt == 0 || deviceData.HeightU == 0)
            {
                DeviceMappingItem target = deviceMappingTable.First(keyValuePair => keyValuePair.Key == deviceData.type).Value;
                deviceData.SetWatt(target.watt);
                deviceData.SetWeight(target.weight);
                deviceData.SetHeightU(target.heightU);
            }
        });
    }

    [Serializable]
    public struct DeviceMappingItem
    {
        public int heightU, watt, weight;
    }
}
