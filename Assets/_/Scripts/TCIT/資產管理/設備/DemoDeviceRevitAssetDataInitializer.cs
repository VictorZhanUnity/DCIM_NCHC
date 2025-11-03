using _VictorDev.TCIT.DCIM;
using UnityEngine;

public class DemoDeviceRevitAssetDataInitializer : MonoBehaviour
{
    private DeviceRevitAssetData deviceRevitAssetData;

    private void Awake()
    {
        deviceRevitAssetData = new DeviceRevitAssetData();
        deviceRevitAssetData.ForDemo(transform);
    }
}
