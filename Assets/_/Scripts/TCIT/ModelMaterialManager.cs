using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.MaterialUtils;

namespace _VictorDev.TCIT.DCIM
{
    public class ModelMaterialManager : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private MaterialReplacer
            materialReplacerRack, materialReplacerDevice, materialReplacerOthers, materialReplacerSectionRacks;

        #endregion

        /// 指定顯示Section機櫃群
        public void ShowSectionRacks(List<Transform> rackModels)
        {
            materialReplacerRack.ReplaceModelsMaterial();
            materialReplacerDevice.ReplaceModelsMaterial();
            materialReplacerOthers.ReplaceModelsMaterial();
            materialReplacerSectionRacks.SetTargetModels(rackModels);
            materialReplacerSectionRacks.RestoreModelsMaterial();
        }

        [Button]
        public void ShowRackAndDevice()
        {
            materialReplacerRack.RestoreModelsMaterial();
            materialReplacerDevice.RestoreModelsMaterial();
            materialReplacerOthers.ReplaceModelsMaterial();
        }

        /// 僅顯示目標物件(包含子物件)
        public void ShowTargetModel(Transform target)
        {
            materialReplacerRack.ReplaceModelsMaterial();
            materialReplacerDevice.ReplaceModelsMaterial();
            materialReplacerOthers.ReplaceModelsMaterial();
            MaterialHelper.RestoreMaterial(target);
        }

        [Button]
        public void ShowAllModels()
        {
            materialReplacerRack.RestoreModelsMaterial();
            materialReplacerDevice.RestoreModelsMaterial();
            materialReplacerOthers.RestoreModelsMaterial();
        }
    }
}