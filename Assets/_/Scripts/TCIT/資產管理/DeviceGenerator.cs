using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.RevitAssetModule
{
    public class DeviceGenerator : MonoBehaviour
    {
        #region Variables

        [Foldout("[Prefab] - 設備模型"), SerializeField] private List<Transform> deviceModels;
        
        
        
        #endregion
    }
}

