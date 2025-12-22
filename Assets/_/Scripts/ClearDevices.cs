using _VictorDev.ApiExtensions;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;

public class ClearDevices : MonoBehaviour
{
    public RevitAssetDataManager revitAssetDataManager;

    [Button]
    private void RemoveDevices()
    {
        revitAssetDataManager.Data.ForEach(rackData=> rackData.Model.RemoveAllChildren());
    }
}
