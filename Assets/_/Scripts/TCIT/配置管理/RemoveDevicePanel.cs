using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class RemoveDevicePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtDeviceNameAndCode;

    private DeviceRevitAssetData deviceData;
    
    
    public void SetDeviceData(DeviceRevitAssetData data)
    {
        deviceData = data;
        txtDeviceNameAndCode.SetText(deviceData.DeviceNameAndCode);
    }

    public void ConfirmRemoveDevice()
    {
        RackRevitAssetData rackRevitAssetData =
            deviceData.Model.parent.GetComponent<RevitAssetDataHolder>().RackRevitData;
        deviceData.Model.Destroy();
        rackRevitAssetData.RemoveDevice(deviceData);
    }
}
