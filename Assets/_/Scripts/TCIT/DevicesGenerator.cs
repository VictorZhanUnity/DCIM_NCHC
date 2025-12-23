using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _VictorDev.TCIT.DCIM.RevitAssetModule
{
    /// 設備動態生成
    public class DevicesGenerator : MonoBehaviour
    {
        #region Variables
        [Label("[設備模型]"), SerializeField] private List<Transform> deviceModels;
        [Label("[Revit模型Prefab]"), SerializeField] private List<Transform> devicePrefabs;
        [Foldout("[組件]"), SerializeField] private RevitAssetDataManager revitAssetDataManager;
        [Foldout("[設定]"), SerializeField] private int chunkSize = 20;

        private Coroutine coroutine;
        #endregion

        /// 動態產生設備
        [Button]
        public void GenerateDevices()
        {
            if(coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(CoroutineHandler());
            IEnumerator CoroutineHandler()
            {
                int counter = 0;
                
                for (int i = 0; i < revitAssetDataManager.Data.Count; i++)
                {
                    RackRevitAssetData rackData = revitAssetDataManager.Data[i];
                    RackUnitGrid rackUnitGrid = rackData.Model.GetComponentInChildren<RackUnitGrid>();
                    
                    for(int j = 0; j < rackData.Containers.Count; j++)
                    {
                        CreateDeviceModel(rackData.Containers[j], rackUnitGrid);
                    }

                    if (++counter > chunkSize)
                    {
                        yield return new WaitForEndOfFrame();
                        counter = 0;
                    }
                }
            }
        }

        private void CreateDeviceModel(DeviceRevitAssetData deviceData, RackUnitGrid rackUnitGrid)
        {
            var targetModel = deviceModels.FirstOrDefault(model=> model.name.ContainKeyword(StringComparison.OrdinalIgnoreCase, deviceData.modelNumber));
            if (targetModel == null)
            {
                Debug.LogWarning($"Cant find target model at location: {deviceData.modelNumber}");
                return;
            }
            Debug.Log($"Create Target Model: {deviceData.modelNumber}");
            Transform newDevice = Instantiate(targetModel.transform, rackUnitGrid.transform.parent);
            deviceData.SetModel(newDevice);
            newDevice.AddComponent<RevitAssetDataHolder>().ReceiveAssetData(deviceData);
            newDevice.AddComponent<BoxCollider>();
            rackUnitGrid.SetDevicePosition(deviceData);
        }

        [Button]
        private void RemoveAllDeviceModels() => revitAssetDataManager.Data.ForEach(rackData=> rackData.Model.RemoveAllChildren("PDU", "RackUnitGridPrefab"));

        private void OnValidate()
        {
            if (devicePrefabs.ClearMissingTargets().Count > 0)
            {
                deviceModels = devicePrefabs.SelectMany(model=> model.GetComponentsInChildren<MeshRenderer>())
                    .Select(x=> x.transform).ToList();
            }
            else
                deviceModels.Clear();
        }
    }
}
