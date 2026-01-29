using System.Collections.Generic;
using System.Linq;
using _VictorDev.Framework.WebAPI;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils.EditableTextComponent;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UploadDeviceInputPanel : MonoBehaviour
{
    public UnityEvent<DeviceRevitAssetData> onConfirmUploadDeviceEvent;
    
    private UploadDeviceRevitAssetData _uploadDeviceRevitAssetData;
    private RackRevitAssetData _rackRevitAssetData;

    public List<EditableText> editableTexts;

    public WebAPICaller uploadWebAPICaller;

    private int currentRackLocation;
    
    public void SetUploadDeviceInfo(UploadDeviceRevitAssetData data)
    {
        _uploadDeviceRevitAssetData = data;
        editableTexts[0].SetText(_uploadDeviceRevitAssetData.DeviceNameAndCode);
    }
    public void SetRackInfo(int rackLocation, RackRevitAssetData data)
    {
        currentRackLocation = rackLocation;
        _rackRevitAssetData = data;
    }

    public void ToUploadDevice()
    {
        var jObj = JObject.Parse(uploadWebAPICaller.SendBodyJson);
        jObj["rackDeviceCode"] = _rackRevitAssetData.deviceCode;
        jObj["containerDeviceCode"] = _uploadDeviceRevitAssetData.deviceCode;
        jObj["rackLocation"] = currentRackLocation.ToString();
        uploadWebAPICaller.SetBodyJson(jObj.ToString());
        uploadWebAPICaller.CallAPI();
    }
    
    public void ConfirmToUploadDevice()
    {
        DeviceRevitAssetData deviceData = new DeviceRevitAssetData
        {
            DeviceNameAndCode = editableTexts[0].Text.Trim(),
            RackLocation = currentRackLocation,
            DeviceName = editableTexts[0].Text.Trim(),
        };
        deviceData.SetDeviceCode(_uploadDeviceRevitAssetData.deviceCode);
        deviceData.Information = new Information()
        {
            watt = _uploadDeviceRevitAssetData.Watt,
            weight = _uploadDeviceRevitAssetData.Weight,
            heightU = _uploadDeviceRevitAssetData.HeightU,
        };
        
        _rackRevitAssetData.Containers.Add(deviceData);
        onConfirmUploadDeviceEvent?.Invoke(deviceData);
    }

    [Button]
    private void OnValidate()
    {
        editableTexts = GetComponentsInChildren<EditableText>().ToList();
    }
}