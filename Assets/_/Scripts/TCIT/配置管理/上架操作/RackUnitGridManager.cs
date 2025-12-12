using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using _VictorDev.GimzoUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    public class RackUnitGridManager : MonoBehaviour, IReceiveData<List<Transform>>
    {
        #region Variables

        [Label("機櫃模型列表"), SerializeField] private List<Transform> rackModelList;

        [Label("RackUnitGrid列表"), SerializeField]
        private List<RackUnitGrid> rackUnitGridList;

        /// U層位置 / Rack資訊 / 上架設備資訊
        [Foldout("[Event] 目前MouseOver的機櫃資訊")] public UnityEvent<int, RackRevitAssetData> onMouseOverEvent;
        [Foldout("[Event] 目前MouseClick的機櫃資訊")] public UnityEvent<int, RackRevitAssetData> onMouseClickEvent;
        [Foldout("[Event] 目前MouseClick的機櫃資訊")] public UnityEvent<Transform> toFocusDeviceModelEvent;
        [Foldout("[Event] 目前MouseClick的機櫃資訊")] public UnityEvent<Transform> onClickRackModelEvent;
        [Foldout("[Event] 取消選擇機櫃(Invoke False)")] public UnityEvent<bool> cancelSelectedRackEvent;

        [Foldout("[設定]"), SerializeField] private string rackKeyWord;
        [Foldout("[設定]"), SerializeField] private RackUnitGrid rackUnitGridPrefab;

        
        /// 欲上架的設備資訊
        private UploadDeviceRevitAssetData uploadDeviceRevitAssetData;
        /// 欲上架的設備模型
        private Transform selectedDevice;

        /// 目前選的機櫃
        private RackRevitAssetData currentRackRevitAssetData;
        /// 目前選的U層
        private int currentPositionU;
        /// 是否已選擇機櫃U層RackUnitGrid
        private bool isSelectedRackUnitGrid;
            
        #endregion

        /// 設定欲上架的設備
        public void SetSelectedDevice(UploadDeviceRevitAssetData data)
        {
            uploadDeviceRevitAssetData = data;
            if(selectedDevice != null) ObjectHelper.Destroy(selectedDevice.gameObject);
            selectedDevice = ObjectHelper.Instantiate(uploadDeviceRevitAssetData.Model, transform);
        }

        /// 點擊RackUnitGrid時，切換為選取狀態並發送資料
        public void OnMouseClickRackUnitGrid()
        {
            if (isSelectedRackUnitGrid) return;
            isSelectedRackUnitGrid = true;
            onMouseClickEvent?.Invoke(currentPositionU, currentRackRevitAssetData);
            toFocusDeviceModelEvent?.Invoke(selectedDevice);
            onClickRackModelEvent?.Invoke(currentRackRevitAssetData.Model);
        }

        /// 取消RackUnitGrid的選取狀態
        public void CancelSelectRackUnitGrid()
        {
            isSelectedRackUnitGrid = false;
            currentPositionU = -1;
            currentRackRevitAssetData = null;
            cancelSelectedRackEvent?.Invoke(isSelectedRackUnitGrid);
        }

        /// 確認上架設備
        public void ConfirmSelectedDeviceModel()
        {
            selectedDevice = null;
            CancelSelectRackUnitGrid();
        }
        
        /// 取消上架的設備模型
        public void CancelSelectUploadDevice()
        {
            if(selectedDevice == null) return;
            ObjectHelper.Destroy(selectedDevice.gameObject);
            selectedDevice = null;
        }

        /// 從RaycastManager接收目前Hit到的RackUnitGrid與其座標
        public void ReceiveMouseOverRackUnitGrid(Transform hitObject, Vector3 worldPosition)
        {
            if (isSelectedRackUnitGrid) return;
            
            if (hitObject.TryGetComponentInChildren(out RackUnitGrid rackUnitGrid))
                rackUnitGrid.ReceiveInteractWorldPosition(worldPosition);
            else
                Debug.LogError($"hitObject {hitObject.name} does not have a RackUnitGrid component", this);
        }

        /// 接收模型
        public void ReceiveData(List<Transform> models)
        {
            rackKeyWord = rackKeyWord.Trim();
            rackModelList = models.Where(model => model.name.Contains(rackKeyWord, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        /// 取得RackUnitGrid資訊
        private void OnGetCurrentGridInfo(Vector3 gridWorldPosition, int positionU, RevitAssetDataHolder rackAssetDataHolder)
        {
            if(currentPositionU == positionU && currentRackRevitAssetData == rackAssetDataHolder.RackRevitData) return;
            currentPositionU = positionU;
            currentRackRevitAssetData = rackAssetDataHolder.RackRevitData;
            
            selectedDevice.position = gridWorldPosition;
            selectedDevice.rotation = rackAssetDataHolder.transform.rotation;
            onMouseOverEvent?.Invoke(positionU, rackAssetDataHolder.RackRevitData);
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
            #if UNITY_EDITOR
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
            #endif
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