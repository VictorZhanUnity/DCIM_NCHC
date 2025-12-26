using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.GimzoUtils
{
    /// 依照Grid的CellSize，繪制Gizmo格數以改變BoxCollider尺吋，並提供GridIndex換算世界座標
    /// <para> + 目前僅支援Rectangle型態的Grid </para>
    [ExecuteAlways]
    [RequireComponent(typeof(Grid), typeof(BoxCollider))]
    public class GridGizmoDrawer : MonoBehaviour
    {
        #region Variables

        [Label("繪製Grid數量"), MinValue(1), SerializeField]
        private Vector3Int amountOfGrids = new(3, 3, 3);

        [Label("單一Grid尺吋"), MinValue(0.0001f), SerializeField]
        private Vector3 gridCellSize = Vector3.one;

        [Foldout("[Event] 換算Grid的世界座標")] public UnityEvent<Vector3> onGetGridWorldPositionEvent;

        [Foldout("[Event] 目前Grid的Index(Vector3Int)")]
        public UnityEvent<Vector3Int> onGetCurrentGridIndexEvent;
        [Foldout("[Event] 目前Grid的Index(string)")]
        public UnityEvent<string> onGetCurrentGridIndexStringEvent;

        [Foldout("[Gizmo設定]"), Label("是否始終顯示Gizmo"), SerializeField]
        private bool isAlwaysDisplayGizmo;
        [Foldout("[Gizmo設定]"), Label("是否顯示GridIndex(較耗資源)"), SerializeField]
        private bool isShowGridIndex = true;
        [Foldout("[Gizmo設定]"), SerializeField, Label("Offset顯示Grid指標")]
        private Vector3 offsetDisplayGridIndex = Vector3.zero;
        [Foldout("[Gizmo設定]"), SerializeField] private Color lineColor = Color.orange;
        [Foldout("[組件]"), SerializeField] private Grid grid;
        [Foldout("[組件]"), SerializeField] private BoxCollider boxCollider;

        /// 換算Grid的世界座標 / 目前Grid的Index(Vector3Int)
        public UnityEvent<Vector3, Vector3Int> OnGetCurrentGridInfoEvent { get; } = new();

        #endregion

        /// 設置繪製Grid數量 (X, Y, Z)軸
        public void SetAmountOfGrids(Vector3Int amount) => amountOfGrids = amount;

        /// 設置單一Grid尺吋
        public void SetCellSizeOfGrid(Vector3 size)
        {
            gridCellSize = size;
#if !UNITY_EDITOR
            grid.cellSize = gridCellSize;
#else
            // 記錄要更新
            pendingUpdate = true;

            // 確保不重複註冊
            EditorApplication.update -= ApplyGridCellSize;
            EditorApplication.update += ApplyGridCellSize;
#endif
        }

        /// 以座標換算Grid世界座標
        public Vector3 ToGridWorldPosition(Vector3 worldPosition)
        {
            // 格子座標
            Vector3Int posOfGridIndex = grid.WorldToCell(worldPosition);

            posOfGridIndex.x = Mathf.Clamp(posOfGridIndex.x, 0, amountOfGrids.x - 1);
            posOfGridIndex.y = Mathf.Clamp(posOfGridIndex.y, 0, amountOfGrids.y - 1);
            posOfGridIndex.z = Mathf.Clamp(posOfGridIndex.z, 0, amountOfGrids.z - 1);

            Vector3 posOfWorld = grid.GetCellCenterWorld(posOfGridIndex); //抓Grid中間座標
            onGetGridWorldPositionEvent?.Invoke(posOfWorld);
            onGetCurrentGridIndexEvent?.Invoke(posOfGridIndex);
            OnGetCurrentGridInfoEvent?.Invoke(posOfWorld, posOfGridIndex);
            onGetCurrentGridIndexStringEvent?.Invoke(posOfGridIndex.ToString());
            return posOfWorld;
        }

        public Vector3 CellIndexToWorldPosition(Vector3Int cellIndex)
        {
            var cellPosition = grid.CellToWorld(cellIndex);
            
            Vector3Int posOfGridIndex = grid.WorldToCell(cellPosition);

            posOfGridIndex.x = Mathf.Clamp(posOfGridIndex.x, 0, amountOfGrids.x - 1);
            posOfGridIndex.y = Mathf.Clamp(posOfGridIndex.y, 0, amountOfGrids.y - 1);
            posOfGridIndex.z = Mathf.Clamp(posOfGridIndex.z, 0, amountOfGrids.z - 1);

            Vector3 posOfWorld = grid.GetCellCenterWorld(posOfGridIndex); //抓Grid中間座標
            return posOfWorld;
        }

        /// 向下對齊父類別的Collider
        [Button]
        public void AlignToParentBottomMesh()
        {
            if (transform.parent == null)
            {
                Debug.LogError($"Parent is null,", this);
                return;
            }
            transform.position = transform.parent.GetComponent<MeshRenderer>().bounds.center;
            
            LayerMask parentLayerMask = LayerMaskHelper.IndexToLayerMask(transform.parent);

            if (parentLayerMask == LayerMaskHelper.IndexToLayerMask(transform))
            {
                Debug.LogError($"LayerMask of Parent and Child are the same.", this);
                return;
            }
            
            //從Y軸+1為起點，開始Racast
            Ray ray = new Ray(transform.position.AddY(1f), Vector3.down);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, parentLayerMask))
            {
                //設定XZ相對座標
                transform.localPosition = new Vector3(
                    transform.localPosition.x-gridCellSize.x*0.5f,
                    hitInfo.point.y,
                    transform.localPosition.z-gridCellSize.z*0.5f
                );
                // 設定Y高度座標
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
            else
            {
                Debug.LogError($"Didn't hit Objects.", this);
            }
        }

#if UNITY_EDITOR
        private bool pendingUpdate;
        private void ApplyGridCellSize()
        {
            if (!pendingUpdate) return;
            pendingUpdate = false;
            EditorApplication.update -= ApplyGridCellSize;
            if(grid != null) grid.cellSize = gridCellSize;
        }
#endif
        
        /// 調整BoxCollider尺吋與位置，對齊Grid
        private void FixColliderToGrid()
        {
            // 計算整體大小, 會以BoxCollider表面的位置計算Grid指標而會計算到43，所以要讓高度減一個高度
            Vector3 totalSize = new Vector3(
                grid.cellSize.x * amountOfGrids.x
                , grid.cellSize.y * amountOfGrids.y
                , grid.cellSize.z * amountOfGrids.z);
            // 設定 BoxCollider 大小
            boxCollider.size = totalSize;
            // 因為 BoxCollider 的中心在中間，所以要讓它往 Grid 的一半方向偏移
            boxCollider.center = new Vector3(totalSize.x * 0.5f, totalSize.y * 0.5f, totalSize.z * 0.5f);
        }

        private void OnValidate()
        {
            if (grid == null) grid = GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            SetCellSizeOfGrid(gridCellSize);
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        #region 畫Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isAlwaysDisplayGizmo || Selection.activeGameObject == gameObject) DrawGrid();
            FixColliderToGrid();
        }

        /// 畫Grid格線
        private void DrawGrid()
        {
            if (grid == null) grid = GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;

            Gizmos.color = lineColor;

            // 暫存舊的矩陣
            Matrix4x4 oldMatrix = Gizmos.matrix;
            // 設定為該物件的本地轉換矩陣
            Gizmos.matrix = transform.localToWorldMatrix;

            for (int x = 0; x < amountOfGrids.x; x++)
            {
                for (int y = 0; y < amountOfGrids.y; y++)
                {
                    for (int z = 0; z < amountOfGrids.z; z++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, z);
                        // 使用本地座標，不要用 GetCellCenterWorld
                        Vector3 localPos = grid.CellToLocalInterpolated(cellPos + Vector3.one * 0.5f);

                        Gizmos.DrawWireCube(localPos, gridCellSize);

                        if (isShowGridIndex)
                            Handles.Label(transform.TransformPoint(localPos + offsetDisplayGridIndex),
                                $"X:{x}\tY:{y}\tZ:{z}");
                    }
                }
            }

            // 恢復矩陣
            Gizmos.matrix = oldMatrix;
        }
#endif
        #endregion
    }
}