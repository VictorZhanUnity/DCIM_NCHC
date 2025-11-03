using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Debug =_VictorDev.DebugUtils.Debug;

namespace VictorDev.TCIT.DCIM
{
    /// [Editor模式] - 繪製機櫃裡的RackUnit尺吋
    [RequireComponent(typeof(Grid))]
    public class RackUnitGridGizmoDisplay : MonoBehaviour
    {
        #region Variables
        [Foldout("[Event]")] public UnityEvent<Vector3> onGetGridPosition = new ();

        [Foldout("[設定]"), Label("RackUnit層數"), SerializeField, Range(19, 46)] private int totalRu = 42;

        [Foldout("[設定]"), Label("強制設備Rotate"), SerializeField]
        private Vector3 forceDeviceRotation = new (-90, 0, -90);
        [Foldout("[設定]"), SerializeField] private bool isReverseXZ;
        [Foldout("[設定]"), SerializeField] private Color gridGizmoColor = Color.green;
        [Foldout("[組件]"), SerializeField] private Grid rackGrid;
        [Foldout("[組件]"), SerializeField] private MeshRenderer meshRenderer;


        public Transform device;
        
        #endregion

        /// 依座標取得Grid位置
        public void GetGridPosition(Vector3 worldPosition)
        {
            Vector3Int gridPos = rackGrid.WorldToCell(worldPosition);
            gridPos.Clamp(new Vector3Int(0, -50, 0), new Vector3Int(0, 50, 0));
            Vector3 result = rackGrid.GetCellCenterWorld(gridPos);
            result.z = rackGrid.CellToWorld(gridPos).z;
            Debug.Log($"gridPos: {gridPos} / result: {result} / worldPosition: {worldPosition}");
            onGetGridPosition?.Invoke(result);

            device.transform.parent = transform.parent;
            device.position = result;
            device.rotation = Quaternion.Euler(forceDeviceRotation);
        }
        
        /// 將Grid的CellSize與scale值同步，Y值固定為U層高度
        [Button]
        private void SyncGridSizeFromScale()
        {
            if (rackGrid != null)
            {
                rackGrid.cellSize = new Vector3(transform.localScale.x, DcimHelper.RackUnitSize.y,
                    transform.localScale.z);
            }
        }
        
        #region Initialized

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            return;
            if (meshRenderer == null) return;

            transform.localScale = new Vector3(rackGrid.cellSize.z, rackGrid.cellSize.y, rackGrid.cellSize.x); ;
            
            Bounds bounds = meshRenderer.bounds;
            Gizmos.DrawWireCube(transform.position, rackGrid.cellSize);


           
            //GizmoHelper.DrawRackUGizmos(bounds, rackGrid.cellSize.y, totalRu, gridGizmoColor);
        }
#endif

        private void Awake() => meshRenderer.enabled = false;

        public MeshRenderer parentMesh;
        
        [Button]
        private void OnValidate()
        {
            return;
            if (rackGrid == null) rackGrid = GetComponent<Grid>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
            
            Debug.Log(parentMesh.bounds.center);
            transform.localPosition= parentMesh.bounds.center;
        }
        #endregion
    }
}