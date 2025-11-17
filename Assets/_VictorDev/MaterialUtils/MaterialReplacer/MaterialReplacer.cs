using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.MaterialUtils
{
    /// 處理3D物件的材質替換
    public class MaterialReplacer: MonoBehaviour
    {
        #region Variables

        [Label("[組件]"), SerializeField] private List<Transform> targetModels;
        [Foldout("[設定]"), SerializeField] private Material replaceMaterial;
        
        #endregion


        public void SetTargetModels(List<Transform> targets)
        {
            targetModels = targets; 
        }
        
        [Button]
        private void ReplaceMaterial()
        {
            MaterialHelper.ReplaceMaterial(targetModels, replaceMaterial);
        }

        [Button]
        private void RestoreMaterial()
        {
            MaterialHelper.RestoreMaterial(targetModels);
        }

       
    }
}