using System;
using _VictorDev.ColorUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 環控資料持有
    [DisallowMultipleComponent]
    public class EnvironmentDataHolder:MonoBehaviour
    {
        #region Variables

        [Foldout("[資料項]"), SerializeField] private EnvironmentData envData;
        
        
        [Foldout("[DoTween設定]"), SerializeField] private float duration = 0.5f;
        [Foldout("[DoTween設定]"), SerializeField] private Ease ease = Ease.OutQuad;
        private Color RackSourceColor => ColorHelper.HexToColor(0x333333);
        public EnvironmentData EnvData => envData;
        
        /// 機櫃資料
        public RackRevitAssetData RackData => rackData ??= GetComponent<RevitAssetDataHolder>().RackRevitData;
        private RackRevitAssetData rackData;
        
        private Material RackMaterial => rackMaterial ??= transform.GetComponent<Renderer>().materials[0];
        private Material rackMaterial;

        private EnumEnvDataType rackDisplayType = EnumEnvDataType.None;
        
        #endregion

        public void SetEnvironmentData(EnvironmentData data)
        {
            envData = data;
            UpdateRackColor();
        }
        private void UpdateRackColor()
        {
            Color targetColor = RackSourceColor;
            float percent;
            switch (rackDisplayType)
            {
                case EnumEnvDataType.RT:
                    percent = DcimSysConfig.CalculateRtPercent(envData.rt);
                    targetColor = DcimSysConfig.GetPercentHeatColor(percent);
                    break;
                case EnumEnvDataType.RH:
                    percent = DcimSysConfig.CalculateRhPercent(envData.rh);
                    targetColor = DcimSysConfig.GetPercentHumidityColor(percent);
                    break;
            }

            RackMaterial.DOKill();
            RackMaterial.DOColor(targetColor, "_BaseColor", duration).SetEase(ease);
        }

        public void SetRackDisplayType(EnumEnvDataType value)
        {
            rackDisplayType = value;
            RackMaterial.DOKill();
            RackMaterial.color = RackSourceColor;
        }
    }
}