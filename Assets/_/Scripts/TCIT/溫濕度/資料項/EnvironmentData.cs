using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule
{
    /// 環控資料
    [Serializable]
    public class EnvironmentData
    {
        [JsonProperty] [field: SerializeField] public string deviceCode { get; protected set; }
        [JsonProperty] [field: SerializeField] public List<EnvironmentTag> tags { get; protected set; }


        #region 額外的欄位

        /// 溫度
        [field:SerializeField] public float RTValue { get; set; }

        /// 濕度
        [field:SerializeField] public float RHValue { get; set; }

        #endregion

        
        public void SetDeviceCode(string value) => deviceCode = value;

        [Serializable]
        public class EnvironmentTag
        {
            public string code;
            public string system;
            public string type;
            public string signalType;
            public string tagDescription;
            public string tagUnit;
            public string name;
            public string tagValueMessage;
            public float value;
            public int status;
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            EnvironmentTag result;
            result = tags.FirstOrDefault(data => data.tagDescription.Contains("溫度") && data.value >= 0);
            RTValue = result != null? result.value: -1;
            result = tags.FirstOrDefault(data => data.tagDescription.Contains("濕度") && data.value >= 0);
            RHValue = result != null? result.value: -1;
        }
    }
}