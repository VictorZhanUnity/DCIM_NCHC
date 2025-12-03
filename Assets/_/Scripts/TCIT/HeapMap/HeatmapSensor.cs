using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.Framework.HeatmapUtils
{
    public abstract class HeatmapSensor<TData>:MonoBehaviour
    {
        [Foldout("[資料項]"), SerializeField] private TData data;
        public TData Data => data;
        
        public void SetData(TData value) => data = value;
    }
}