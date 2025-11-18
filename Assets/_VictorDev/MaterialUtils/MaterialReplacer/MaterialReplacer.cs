using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.MaterialUtils
{
    /// 處理3D物件的材質替換
    public class MaterialReplacer: MonoBehaviour
    {
        #region Variables

        [Label("[設備模型]"), SerializeField] private List<Transform> rackModels;
        [Label("[機櫃模型]"), SerializeField] private List<Transform> deviceModels;
        [Label("[其它模型]"), SerializeField] private List<Transform> otherModels;
        [Foldout("[設定]"), Label("[機櫃關鍵字]"), SerializeField] private string[] rackKeywords = new []{"Rack"};
        [Foldout("[設定]"), Label("[設備關鍵字]"), SerializeField] private string[] deviceKeywords = new []{"Server", "Router", "Switch"};
        [Foldout("[設定]"), SerializeField] private Material replaceMaterial;
        [Foldout("[設定]"), SerializeField] private Transform targetModel;
        
        #endregion

        #region For Editor
        [Button]
        private void FindRackModels() => rackModels = targetModel.FindChildrenByKeywords(EnumSearchType.Include, rackKeywords);
        [Button]
        private void FindDeviceModels() => deviceModels = targetModel.FindChildrenByKeywords(EnumSearchType.Include, deviceKeywords);
        [Button]
        private void FindOtherModels() => otherModels = targetModel.FindChildrenByKeywords(EnumSearchType.Exclude
            , rackKeywords.Combine(deviceKeywords));
        #endregion

        [Button]
        public void ShowRackAndDevice()
        {
            MaterialHelper.RestoreMaterial(rackModels);
            MaterialHelper.RestoreMaterial(deviceModels);
            MaterialHelper.ReplaceMaterial(otherModels, replaceMaterial);
        }
        
        /// 僅顯示目標物件
        public void ShowTargetModel(Transform target)
        {
            MaterialHelper.ReplaceMaterial(rackModels, replaceMaterial);
            MaterialHelper.ReplaceMaterial(deviceModels, replaceMaterial);
            MaterialHelper.ReplaceMaterial(otherModels, replaceMaterial);
            MaterialHelper.RestoreMaterial(new List<Transform>() { target });
        }

        [Button]
        public void RestoreAllModelsMaterial()
        {
            MaterialHelper.RestoreMaterial(rackModels);
            MaterialHelper.RestoreMaterial(deviceModels);
            MaterialHelper.RestoreMaterial(otherModels);
        }
    }
}