using System.Collections.Generic;
using System.Linq;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using NaughtyAttributes;
using UnityEngine;
using VictorDev.MaterialUtils;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    public class ModelMaterialManager : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private MaterialReplacer
            materialReplacerRack, materialReplacerDevice, materialReplacerOthers, materialReplacerCustom;

        #endregion


        public List<Transform> devices;
        
        /// 指定顯示Section機櫃群
        public void ShowSectionRacks(EnvironmentSection section)
        {
            materialReplacerRack.ReplaceModelsMaterial();
            materialReplacerDevice.ReplaceModelsMaterial();
            materialReplacerOthers.ReplaceModelsMaterial();

            var holders = section.EvnDataHolders;
            devices = holders.SelectMany(envHolder=>  envHolder.RackData.Containers).Select(device => device.Model).ToList();

            materialReplacerCustom.SetTargetModels(section.RacksAndDevices);
            materialReplacerCustom.RestoreModelsMaterial();
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