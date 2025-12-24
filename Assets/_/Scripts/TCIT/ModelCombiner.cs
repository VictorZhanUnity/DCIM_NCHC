using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using NaughtyAttributes;
using UnityEngine;


public class ModelCombiner : MonoBehaviour
{
   #region Variables
   [Label("[設備模型]"), SerializeField] private List<Transform> deviceModels;
   [Label("[模型Prefab]"), SerializeField] private List<Transform> modelPrefabs;
   
   public List<Transform> DeviceModels => deviceModels;
   
   #endregion

   private void OnValidate()
   {
      if (modelPrefabs != null && modelPrefabs.ClearMissingTargets().Count > 0)
      {
         deviceModels = modelPrefabs.SelectMany(model=> model.GetComponentsInChildren<MeshRenderer>())
            .Select(x=> x.transform).ToList();
      }
      else deviceModels?.Clear();
   }
}
