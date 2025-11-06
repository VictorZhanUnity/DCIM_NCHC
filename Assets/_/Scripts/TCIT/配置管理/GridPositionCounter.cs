using _VictorDev.GimzoUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT
{
    /// 計算GridIndex與世界座標
    [RequireComponent(typeof(Grid), typeof(GridGizmoDrawer), typeof(BoxCollider))]
    public class GridPositionCounter : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event] 換算Grid的世界座標")] public UnityEvent<Vector3> toGridWorldPositionEvent;
        [Foldout("[Event] 目前Grid的Index")] public UnityEvent<Vector3Int> currentGridIndexEvent;

        [Foldout("[組件]"), SerializeField] private Grid grid;
        [Foldout("[組件]"), SerializeField] private GridGizmoDrawer gridGizmoDrawer;
        [Foldout("[組件]"), SerializeField] private BoxCollider boxCollider;

        private Vector3Int AmountOfGrids => gridGizmoDrawer.AmountOfGrids;
        
        #endregion

        /// 以座標換算Grid世界座標
        public void ToGridWorldPosition(Vector3 worldPosition)
        {
            // 格子座標
            Vector3Int posOfGridIndex = grid.WorldToCell(worldPosition);

            posOfGridIndex.x = Mathf.Clamp(posOfGridIndex.x, 0, AmountOfGrids.x-1);
            posOfGridIndex.y = Mathf.Clamp(posOfGridIndex.y, 0, AmountOfGrids.y-1);
            posOfGridIndex.z = Mathf.Clamp(posOfGridIndex.z, 0, AmountOfGrids.z-1);
            
            Vector3 posOfWorld = grid.GetCellCenterWorld(posOfGridIndex);
            toGridWorldPositionEvent?.Invoke(posOfWorld);
            currentGridIndexEvent?.Invoke(posOfGridIndex);
        }

        /// 調整BoxCollider尺吋與位置，對齊Grid
        private void FixColliderToGrid()
        {
            // 計算整體大小, 會以BoxCollider表面的位置計算Grid指標而會計算到43，所以要讓高度減一個高度
            Vector3 totalSize = new Vector3(
                grid.cellSize.x * AmountOfGrids.x
                , grid.cellSize.y * AmountOfGrids.y
                , grid.cellSize.z * AmountOfGrids.z);
            // 設定 BoxCollider 大小
            boxCollider.size = totalSize;
            // 因為 BoxCollider 的中心在中間，所以要讓它往 Grid 的一半方向偏移
            boxCollider.center = new Vector3(totalSize.x * 0.5f, totalSize.y * 0.5f, totalSize.z * 0.5f);
        }
        
        private void OnDrawGizmos() => FixColliderToGrid();

        private void OnValidate()
        {
            if (grid == null) grid = GetComponent<Grid>();
            if (gridGizmoDrawer == null) gridGizmoDrawer = GetComponent<GridGizmoDrawer>();
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
    }
}