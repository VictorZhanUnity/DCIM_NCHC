using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using _VictorDev.GimzoUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.MediatorUtils.Debug;

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
        [Foldout("[Event]")] public UnityEvent<bool> isAbleToPlaceDeviceEvent;

        [Foldout("[設定]"), SerializeField] private string rackKeyWord;
        [Foldout("[設定]"), SerializeField] private RackUnitGrid rackUnitGridPrefab;
        [Foldout("[設定]"), SerializeField] private LayerMask selectableLayerMask;

        
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
        private bool isAbleToPlaceDevice;
            
        #endregion

        /// 設定欲上架的設備
        public void SetSelectedDevice(UploadDeviceRevitAssetData data)
        {
            uploadDeviceRevitAssetData = data;
            if(selectedDevice != null) ObjectHelper.Destroy(selectedDevice.gameObject);
            selectedDevice = Instantiate(uploadDeviceRevitAssetData.Model, transform);
            selectedDevice.gameObject.SetActive(false);
        }

        /// 點擊RackUnitGrid時，切換為選取狀態並發送資料
        public void OnMouseClickRackUnitGrid()
        {
            if (isSelectedRackUnitGrid) return;
            if (isAbleToPlaceDevice == false) return;
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
        public void ConfirmSelectedDeviceModel(DeviceRevitAssetData deviceData)
        {
          //  deviceData.RackLocation = currentPositionU;
            //deviceData.SetDevicePath(selectedDevice.name);
            //deviceData.SetDeviceName(selectedDevice.name.Split("+")[2]);
            deviceData.SetModel(selectedDevice);
            deviceData.RevitAssetKind = DcimHelper.GetDeviceKind(selectedDevice.name);
            selectedDevice.AddComponent<RevitAssetDataHolder>().ReceiveAssetData(deviceData);
            selectedDevice.AddComponent<BoxCollider>();
            selectedDevice.parent = currentRackRevitAssetData.Model.transform;
            selectedDevice.gameObject.SetLayerMask(selectableLayerMask);
            currentRackRevitAssetData.AddDevice(deviceData);
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
            if (currentPositionU == positionU && currentRackRevitAssetData == rackAssetDataHolder.RackRevitData)
            {
              //  selectedDevice.gameObject.SetActive(false);
                return;
            }
            
            //檢查positionU是否可以放置Upload設備
            if (rackAssetDataHolder.RackRevitData.IsAbleToPlaceDevice(positionU, uploadDeviceRevitAssetData.HeightU) ==
                false)
            {
               selectedDevice.gameObject.SetActive(false);
               isAbleToPlaceDevice = false;
               isAbleToPlaceDeviceEvent?.Invoke(isAbleToPlaceDevice);
                return;
            }
            isAbleToPlaceDevice = true; 
            isAbleToPlaceDeviceEvent?.Invoke(isAbleToPlaceDevice);
            selectedDevice.gameObject.SetActive(true);
            
            currentPositionU = positionU;
            currentRackRevitAssetData = rackAssetDataHolder.RackRevitData;

           // gridWorldPosition += Vector3.up * deviceOffsetPosY;

            //Z軸前後偏移
            MeshRenderer deviceMesh = selectedDevice.GetComponent<MeshRenderer>();
            MeshRenderer rackMesh = rackAssetDataHolder.RackRevitData.ModelMeshRender;
            /*float deviceOffsetZ = deviceMesh.bounds.center.x - deviceMesh.bounds.min.x;
            float rackOffsetZ = rackMesh.bounds.center.x - rackMesh.bounds.min.x;
            gridWorldPosition -= Vector3.left * (rackOffsetZ-deviceOffsetZ+deviceOffsetPosZ);*/
            
          //  selectedDevice.rotation = rackAssetDataHolder.transform.rotation;
            selectedDevice.SetParent(rackAssetDataHolder.RackRevitData.Model, true);
            selectedDevice.SetPositionByBottom(gridWorldPosition);
            selectedDevice.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            selectedDevice.ToParentFront();
            onMouseOverEvent?.Invoke(positionU, rackAssetDataHolder.RackRevitData);
        }

        [SerializeField] private float deviceOffsetPosY;
        [SerializeField] private float deviceOffsetPosZ;
        

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