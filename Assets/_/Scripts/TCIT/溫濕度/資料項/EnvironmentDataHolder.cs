using System;
using _VictorDev.ApiExtensions;
using _VictorDev.ColorUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    /// 環控資料持有
    [DisallowMultipleComponent]
    public class EnvironmentDataHolder:MonoBehaviour
    {
        #region Variables

        [Label("[資料項] - EnvironmentData"), SerializeField] private EnvironmentData envData;
        
        
        [Foldout("[DoTween設定]"), SerializeField] private float duration = 0.5f;
        [Foldout("[DoTween設定]"), SerializeField] private Ease ease = Ease.OutQuad;
        private Color rackSourceColor;
        public EnvironmentData EnvData => envData;
        
        /// 機櫃資料
        public RackRevitAssetData RackData => rackData ??= GetComponent<RevitAssetDataHolder>().RackRevitData;
        private RackRevitAssetData rackData;
        
        private Material RackMaterial => rackMaterial ??= transform.GetComponent<Renderer>().materials[0];
        private Material rackMaterial;

        private EnumEnvDataType rackDisplayType = EnumEnvDataType.None;
        private Tween tween;
        
        #endregion

        private void Start() => rackSourceColor = RackMaterial.color;

        public void SetEnvironmentData(EnvironmentData data)
        {
            envData = data;
          // if(Application.isPlaying) UpdateRackColor();
        }

        public void UpdateDeviceCode()
        {
            var revitDataHolder = GetComponent<RevitAssetDataHolder>();
            if(revitDataHolder != null) envData.SetDeviceCode(revitDataHolder.RackRevitData.deviceCode);
            else Debug.LogWarning($"RevitAssetDataHolder component not found\n{gameObject.name}");
        }
        
        private void UpdateRackColor()
        {
            Color targetColor = rackSourceColor;
            float percent;
            switch (rackDisplayType)
            {
                case EnumEnvDataType.RT:
                    percent = DcimSysConfig.RTValueRange.GetPercentage01(envData.RTValue);
                    targetColor = DcimSysConfig.GetPercentHeatColor(percent);
                    break;
                case EnumEnvDataType.RH:
                    percent = DcimSysConfig.RTValueRange.GetPercentage01(envData.RHValue);
                    targetColor = DcimSysConfig.GetPercentHumidityColor(percent);
                    break;
            }

            tween?.Kill();
            tween = RackMaterial.DOColor(targetColor, "_BaseColor", duration).SetEase(ease);
        }

        public void SetRackDisplayType(EnumEnvDataType value)
        {
            rackDisplayType = value;
            tween?.Kill();
            RackMaterial.color = rackSourceColor;
        }
    }
}