using NaughtyAttributes;
using UnityEngine;
using VictorDev.MaterialUtils;

namespace _VictorDev.TCIT.DCIM
{
    public class ModelMaterialManager : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private MaterialReplacer
            materialReplacerRack, materialReplacerDevice, materialReplacerOthers;

        #endregion


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
        public void RestoreAllModelsMaterial()
        {
            materialReplacerRack.RestoreModelsMaterial();
            materialReplacerDevice.RestoreModelsMaterial();
            materialReplacerOthers.RestoreModelsMaterial();
        }
    }
}