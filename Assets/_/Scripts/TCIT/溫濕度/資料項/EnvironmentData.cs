using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控資料
    [Serializable]
    public class EnvironmentData
    {
        public float rt, rh;
       
    }
}