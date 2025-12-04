using _VictorDev.ApiExtensions;
using _VictorDev.TCIT.DCIM;
using UnityEngine;

namespace _VictorDev.Framework.HeatmapUtils
{
    public class HeatmapSensor_Environment: HeatmapSensor<EnvironmentDataHolder>
    {
        public float RT => Data.EnvData.rt;
        public float RH => Data.EnvData.rh;
        
        public Vector4 Vector4Data_RT => GetVector4Data(DcimSysConfig.RTValueValueRange.GetPercentage01(RT));
        public Vector4 Vector4Data_RH => GetVector4Data(DcimSysConfig.RhValueValueRange.GetPercentage01(RH));

        private Vector3? centerPos;
        
        private Vector4 GetVector4Data(float value)
        {
            centerPos ??= Data.transform.GetComponent<MeshRenderer>().bounds.center;
            return new Vector4(centerPos.Value.x, centerPos.Value.z, value,  Random.value * 10f);
        }
    }
}