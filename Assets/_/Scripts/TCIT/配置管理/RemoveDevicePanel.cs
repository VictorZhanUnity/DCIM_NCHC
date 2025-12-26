using _VictorDev.ApiExtensions;
using _VictorDev.Framework.WebAPI;
using _VictorDev.TCIT.DCIM;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class RemoveDevicePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtDeviceNameAndCode;

    private DeviceRevitAssetData deviceData;
    
    public WebAPICaller uninstallWebAPICaller;
    
    public void SetDeviceData(DeviceRevitAssetData data)
    {
        deviceData = data;
        txtDeviceNameAndCode.SetText(deviceData.DeviceNameAndCode);
    }

    public void ToUploadDevice()
    {
        RackRevitAssetData rackRevitAssetData = deviceData.Model.parent.GetComponent<RevitAssetDataHolder>().RackRevitData;
        var jObj = JObject.Parse(uninstallWebAPICaller.SendBodyJson);
        jObj["rackDeviceCode"] = rackRevitAssetData.deviceCode;
        jObj["containerDeviceCode"] = deviceData.deviceCode;
        jObj["rackLocation"] = deviceData.RackLocation.ToString();
        uninstallWebAPICaller.SetBodyJson(jObj.ToString());
        uninstallWebAPICaller.CallAPI();
    }
    
    public void ConfirmRemoveDevice()
    {
        RackRevitAssetData rackRevitAssetData =
            deviceData.Model.parent.GetComponent<RevitAssetDataHolder>().RackRevitData;
        deviceData.Model.Destroy();
        rackRevitAssetData.RemoveDevice(deviceData);
    }
}
