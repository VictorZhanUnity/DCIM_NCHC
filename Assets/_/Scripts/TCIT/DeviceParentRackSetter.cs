using System;
using System.Collections.Generic;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    public class DeviceParentRackSetter : MonoBehaviour, IReceiveData<List<RackRevitAssetData>>
    {
        #region MyRegion
        [Label("[資料項]"), SerializeField] private List<RackRevitAssetData> rackData;
        #endregion
        
        public void ReceiveData(List<RackRevitAssetData> data) => rackData = data;

        [Button]
        private void SetDevicesParentRack()
        {
            rackData.ForEach(rack => rack.Containers.ForEach(device=>device.Model.parent = rack.Model));    
        }

        private void OnValidate()
        {
            name = $"[Editor] - {GetType().Name}";
        }
    }
}
