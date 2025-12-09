using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DateTimeUtils;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.Framework.HeatmapUtils
{
    public class HeatmapManager : MonoBehaviour, ITimerUpdate
    {
        #region Variables

        [Label("[機櫃Sensor群]"), SerializeField] private List<HeatmapSensor_Environment> rackSensors;
        [Foldout("[Event] 顯示熱雲圖")] public UnityEvent onShowHeatmapEvent;
        [Foldout("[組件]"), SerializeField] private GameObject heatmapPlane;
        [Foldout("[設定]"), SerializeField] private Material heatmapMaterial;
        [Foldout("[設定]"), SerializeField] private Texture2D textureRt, textureRh;
        [Foldout("[設定]"), SerializeField] private MeshRenderer targetMeshRenderer;
        [Foldout("[耦合]"), SerializeField] private EnvironmentDataManager envDataManager;
        
        private Vector4[] currentSensorArray, targetSensorArray;

        private string StrPointCount => "_PointCount";
        private string StrPoints => "_Points";
        private string StrGradientTexture => "_GradientTex";

        private EnumEnvDataType envDataType;
        #endregion

        [Button]
        private void CreateSensorToRackModels()
        {
            rackSensors.Clear();
            envDataManager.DataHolders.ForEach(holder =>
            {
                HeatmapSensor_Environment sensor = holder.transform.TryAddComponent<HeatmapSensor_Environment>();
                sensor.SetData(holder);
                rackSensors.Add(sensor);
            });
            rackSensors = rackSensors.OrderBy(target => target.name).ToList();
        }

        [Button]
        private void RemoveSensorFromRackModels()
        {
            rackSensors.ForEach(ObjectHelper.Destroy);
            rackSensors.Clear();
        }

        [Button]
        public void ShowHeatmap_RT()
        {
            envDataType = EnumEnvDataType.RT;
            heatmapMaterial.SetTextureURP(textureRt, StrGradientTexture);
            targetSensorArray = rackSensors.Select(target => target.Vector4Data_RT).ToArray();
            
            TweenSensorValues();
            heatmapPlane.SetActive(true);
            onShowHeatmapEvent?.Invoke();
        }
        [Button]
        public void ShowHeatmap_RH()
        {
            envDataType = EnumEnvDataType.RH;
            heatmapMaterial.SetTextureURP(textureRh, StrGradientTexture);
            targetSensorArray = rackSensors.Select(target => target.Vector4Data_RH).ToArray();
            
            TweenSensorValues();
            heatmapPlane.SetActive(true);
            onShowHeatmapEvent?.Invoke();
        }

        /// 補間更新所有點位值
        private void TweenSensorValues()
        {
            // ⬇️ 第一次初始化 currentSensorArray
            if (currentSensorArray == null)
            {
                currentSensorArray = new Vector4[targetSensorArray.Length];
                Array.Copy(targetSensorArray, currentSensorArray, targetSensorArray.Length);
            }
            
            float duration = 1f;

            for (int i = 0; i < currentSensorArray.Length; i++)
            {
                int index = i;

                DOTween.To(
                    () => currentSensorArray[index],
                    v =>
                    {
                        currentSensorArray[index] = v;
                        heatmapMaterial.SetVectorArray(StrPoints, currentSensorArray);
                    },
                    targetSensorArray[index],
                    duration
                ).SetEase(Ease.InOutSine);
            }
        }

        /*
        private void SetHeatmapData()
        {
            heatmapMaterial.SetInt(StrPointCount, currentSensorArray.Length);
            heatmapMaterial.SetVectorArray(StrPoints, currentSensorArray);
        }
        */

        private void Awake()
        {
            if (targetMeshRenderer != null) targetMeshRenderer.material = heatmapMaterial;
            if (heatmapPlane != null) heatmapPlane = transform.GetChild(0).gameObject;
        }

        private void OnValidate() => Awake();

        public void OnTimeUpdate()
        {
            switch (envDataType)
            {
                case EnumEnvDataType.RT: ShowHeatmap_RT(); break;
                case EnumEnvDataType.RH: ShowHeatmap_RH(); break;
            }
        }

        public void OnTimeFinished()
        {
        }
    }
}