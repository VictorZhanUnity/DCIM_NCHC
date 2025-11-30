using _VictorDev.TCIT.DCIM.EnvironmentModule;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 環控資料持有
    [DisallowMultipleComponent]
    public class EnvironmentDataHolder:MonoBehaviour
    {        
        [field: SerializeField] public RackRevitAssetData RackData { get; private set; }
        [field: SerializeField] public EnvironmentData EnvData { get; private set; }
        
        public void SetRackData(RackRevitAssetData data) => RackData = data;
        public void SetEnvironmentData(EnvironmentData data) => EnvData = data;
    }
}