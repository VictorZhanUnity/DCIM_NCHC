using _VictorDev.TCIT.DCIM;
using UnityEngine;

namespace _VictorDev.Framework.HeatmapUtils
{
    public class HeatmapSensor_Environment: HeatmapSensor<EnvironmentDataHolder>
    {
        public float RT => Data.EnvData.rt;
        public float RH => Data.EnvData.rh;
        
        public Vector4 Vector4Data_RT => GetVector4Data(DcimSysConfig.CalculateRtPercent(RT));
        public Vector4 Vector4Data_RH => GetVector4Data(DcimSysConfig.CalculateRhPercent(RH));
        
        //private Vector4 GetVector4Data(float value) => new Vector4(transform.position.x, transform.position.y, value, 0);
        private Vector4 GetVector4Data(float value)
        {
            Debug.Log($"value: {new Vector4(transform.position.x, transform.position.z, value, 0)}");
            return new Vector4(transform.position.x, transform.position.z, value, 0);
        }
    }
}