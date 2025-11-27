using System.Collections.Generic;
using _VictorDev.InterfaceUtils;
using _VictorDev.DebugUtils;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    public class EnvironmentDataManager : JsonDataManagerParent<EnvironmentData>, IReceiveData<List<RackRevitAssetData>>
    {
        
        
        /// 暫時性
        public void ReceiveData(List<RackRevitAssetData> data)
        {
            Debug.Log(data.Count);
        }
    }
}



