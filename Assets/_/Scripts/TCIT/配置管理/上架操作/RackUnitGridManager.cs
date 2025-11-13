using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using _VictorDev.GimzoUtils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    public class RackUnitGridManager : MonoBehaviour
    {
        #region Variables

        [Label("機櫃模型列表"), SerializeField] private List<Transform> rackModelList;

        [Label("RackUnitGrid列表"), SerializeField]
        private List<RackUnitGrid> rackUnitGridList;

        /// U層位置 / Rack資訊 / 上架設備資訊
        [Foldout("[Event] 目前預選的上架資訊")]
        public UnityEvent<int, RevitAssetDataHolder, Transform> onGetCurrentGridInfoEvent;

        [Foldout("[設定]"), SerializeField] private string rackKeyWord;
        [Foldout("[設定]"), SerializeField] private RackUnitGrid rackUnitGridPrefab;

        /// 欲上架的設備資訊
        public Transform selectedDevice;

        ///是否已選擇機櫃的U層位置
        public bool isSelectedRackU;

        #endregion

        /// 設定欲上架的設備
        public void SetSelectedDevice(UploadDeviceRevitAssetData uploadDeviceRevitAssetData)
        {
            if(selectedDevice != null) ObjectHelper.Destroy(selectedDevice.gameObject);
            selectedDevice = ObjectHelper.Instantiate(uploadDeviceRevitAssetData.Model, transform);
        }

        public void SetIsSelectedRackU(bool isSelected)
        {
            isSelectedRackU = isSelected;
        }

        /// 從RaycastManager接收目前Hit到的RackUnitGrid與其座標
        public void ReceiveMouseOverRackUnitGrid(Transform hitObject, Vector3 worldPosition)
        {
            if (selectedDevice == null || isSelectedRackU) return; //若無選取設備上架時則return
            
            if (hitObject.TryGetComponentInChildren(out RackUnitGrid rackUnitGrid))
                rackUnitGrid.ReceiveInteractWorldPosition(worldPosition);
            else
                Debug.LogError($"hitObject {hitObject.name} does not have a RackUnitGrid component", this);
        }

        /// 接收模型
        public void ReceiveModels(List<Transform> models)
        {
            rackKeyWord = rackKeyWord.Trim();
            rackModelList = models.Where(model => model.name.Contains(rackKeyWord, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private int currentPositionU;
        private RackRevitAssetData currentRackRevitAssetData;
        
        /// 取得RackUnitGrid資訊
        private void OnGetCurrentGridInfo(Vector3 gridWorldPosition, int positionU,
            RevitAssetDataHolder rackAssetDataHolder)
        {
            if(currentPositionU == positionU && currentRackRevitAssetData == rackAssetDataHolder.RackRevitData) return;
            currentPositionU = positionU;
            currentRackRevitAssetData = rackAssetDataHolder.RackRevitData;
            
            selectedDevice.position = gridWorldPosition;
            selectedDevice.rotation = rackAssetDataHolder.transform.rotation;
            onGetCurrentGridInfoEvent?.Invoke(positionU, rackAssetDataHolder, selectedDevice);
        }

        #region Initialized

        private void OnEnable() => rackUnitGridList.ForEach(rackUnitGrid =>
            rackUnitGrid.OnGetCurrentGridInfoEvent.AddListener(OnGetCurrentGridInfo));

        private void OnDisable() => rackUnitGridList.ForEach(rackUnitGrid =>
            rackUnitGrid.OnGetCurrentGridInfoEvent.RemoveListener(OnGetCurrentGridInfo));

        #endregion

        #region For Editor

        /// 新增RackUnitGrid到機櫃模型下
        [Button]
        private void BuildRackUnitGridToModels()
        {
            RemoveRackUnitGridFromModels();
            rackModelList.ForEach(rackModel =>
            {
                RackUnitGrid rackUnitGrid = rackModel.GetComponentInChildren<RackUnitGrid>();
                if (rackUnitGrid == null)
                {
                    rackUnitGrid = (RackUnitGrid)PrefabUtility.InstantiatePrefab(rackUnitGridPrefab, rackModel);
                    rackUnitGrid.GetComponent<GridGizmoDrawer>().AlignToParentBottomMesh();
                    rackUnitGridList.Add(rackUnitGrid);
                }
            });
        }

        /// 將RackUnitGrid從機櫃模型裡移除
        [Button]
        private void RemoveRackUnitGridFromModels()
        {
            rackModelList.ForEach(rackModel =>
            {
                RackUnitGrid rackUnitGrid = rackModel.GetComponentInChildren<RackUnitGrid>();
                if (rackUnitGrid != null) ObjectHelper.Destroy(rackUnitGrid.gameObject);
            });
            rackUnitGridList.Clear();
        }

        #endregion
    }
}