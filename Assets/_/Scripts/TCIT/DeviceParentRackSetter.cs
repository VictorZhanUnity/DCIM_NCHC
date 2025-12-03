using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    public class DeviceParentRackSetter : MonoBehaviour
    {
        #region MyRegion
        [SerializeField] private RevitAssetDataManager revitAssetDataManager;
        #endregion
        
        

        [Button]
        private void SetDevicesParentRack()
        {
            revitAssetDataManager.Data.ForEach(rack => rack.Containers.ForEach(device=>device.Model.parent = rack.Model));    
        }

        private void OnValidate()
        {
            name = $"[Editor] - {GetType().Name}";
            revitAssetDataManager = FindAnyObjectByType<RevitAssetDataManager>();
        }
    }
}
