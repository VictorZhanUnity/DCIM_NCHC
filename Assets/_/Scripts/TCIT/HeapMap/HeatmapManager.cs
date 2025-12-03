using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.Framework.HeatmapUtils
{
    public class HeatmapManager : MonoBehaviour
    {
        #region Variables

        [Label("[Sensor群]"), SerializeField] private List<HeatmapSensor_Environment> sensors;
        [Foldout("[設定]"), SerializeField] private Material heatmapMaterial;
        [Foldout("[設定]"), SerializeField] private Texture2D textureRt, textureRh;
        [Foldout("[設定]"), SerializeField] private MeshRenderer targetMeshRenderer;
        [Foldout("[設定]"), SerializeField] private EnvironmentDataManager envDataManager;

        private Vector4[] sensorArray;

        private string StrPointCount => "_PointCount";
        private string StrPoints => "_Points";
        private string StrGradientTexture => "_GradientTex";
        #endregion

        [Button]
        private void CreateSensorToRackModels()
        {
            sensors.Clear();
            envDataManager.DataHolders.ForEach(holder =>
            {
                HeatmapSensor_Environment sensor = holder.transform.TryAddComponent<HeatmapSensor_Environment>();
                sensor.SetData(holder);
                sensors.Add(sensor);
            });
            sensors = sensors.OrderBy(target => target.name).ToList();
        }

        [Button]
        private void RemoveSensorFromRackModels()
        {
            sensors.ForEach(ObjectHelper.Destroy);
            sensors.Clear();
        }

        [Button]
        public void ShowHeatmap_RT()
        {
            heatmapMaterial.SetTextureURP(textureRt, StrGradientTexture);
            sensorArray = sensors.Select(target => target.Vector4Data_RT).ToArray();
            SetHeatmapData();
        }
        [Button]
        public void ShowHeatmap_RH()
        {
            heatmapMaterial.SetTextureURP(textureRh, StrGradientTexture);
            sensorArray = sensors.Select(target => target.Vector4Data_RH).ToArray();
            SetHeatmapData();
        }

        private void SetHeatmapData()
        {
            heatmapMaterial.SetInt(StrPointCount, sensorArray.Length);
            heatmapMaterial.SetVectorArray(StrPoints, sensorArray);
        }

        private void Awake()
        {
            if (targetMeshRenderer != null) targetMeshRenderer.material = heatmapMaterial;
        }

        private void OnValidate() => Awake();
    }
}