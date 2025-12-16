using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils;
using _VictorDev.TextUtils.EditableTextComponent;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class UploadDeviceInputPanel : MonoBehaviour
{
    public UnityEvent<DeviceRevitAssetData> onConfirmUploadDeviceEvent;
    
    private UploadDeviceRevitAssetData _uploadDeviceRevitAssetData;
    private RackRevitAssetData _rackRevitAssetData;

    public List<EditableText> editableTexts;
    
    
    public void SetUploadDeviceInfo(UploadDeviceRevitAssetData data)
    {
        _uploadDeviceRevitAssetData = data;
        editableTexts[0].SetText(_uploadDeviceRevitAssetData.DeviceNameAndCode);
    }
    public void SetRackInfo(int rackLocation, RackRevitAssetData data)
    {
        _rackRevitAssetData = data;
    }

    public void ConfirmToUploadDevice()
    {
        DeviceRevitAssetData deviceData = new DeviceRevitAssetData
        {
            DeviceNameAndCode = editableTexts[0].Text.Trim(),
        };
        deviceData.Information = new Information()
        {
            watt = _uploadDeviceRevitAssetData.Watt,
            weight = _uploadDeviceRevitAssetData.Weight,
            heightU = _uploadDeviceRevitAssetData.HeightU,
        };
        
        onConfirmUploadDeviceEvent?.Invoke(deviceData);
    }

    [Button]
    private void OnValidate()
    {
        editableTexts = GetComponentsInChildren<EditableText>().ToList();
    }
}
