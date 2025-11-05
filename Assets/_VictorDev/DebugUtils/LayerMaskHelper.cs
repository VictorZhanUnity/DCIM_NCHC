using UnityEngine;

namespace _VictorDev.DebugUtils
{
    public static class LayerMaskHelper
    {
        /// 取得LayerMask名稱
        public static LayerMask GetLayerMask(int layerIndex) => 1 << layerIndex;
        public static LayerMask GetLayerMask(Transform target) => GetLayerMask(target.gameObject.layer);
    }
}