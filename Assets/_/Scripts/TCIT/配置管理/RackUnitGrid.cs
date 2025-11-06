using _VictorDev.ApiExtensions;
using _VictorDev.GimzoUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace _VictorDev.TCIT
{
    /// [配置管理] - 機櫃空間Grid
    [RequireComponent(typeof(Grid), typeof(BoxCollider))]
    public class RackUnitGrid : MonoBehaviour
    {
        #region Variables

        [SerializeField, Range(1, 49), Label("機櫃U層數")] private int rackUnits = 42;
        [Foldout("[Event] 換算Grid座標")] public UnityEvent<Vector3> gridToWorldPositionEvent;
        [Foldout("[Event] 目前第幾U")] public UnityEvent<int> currentHeightUEvent;
        [Foldout("[設定]"), SerializeField, Min(0.0001f), Label("單一U層尺吋")] private Vector3 rackUnitSize = DcimHelper.RackUnitSize;
        [Foldout("[設定]"), SerializeField, Min(1), Label("寬度/深度格數")] private Vector2 amountOfWidthDepth = Vector2.one;
        [Foldout("[組件]"), SerializeField] private Grid grid;
        [Foldout("[組件]"), SerializeField] private BoxCollider boxCollider;
        [Foldout("[組件]"), SerializeField] private BoxGridGizmoDrawer boxGridGizmoDrawer;

        #endregion

        /// 以座標換算Grid世界座標
        public void ToGridPosition(Vector3 worldPosition)
        {
            // 格子座標
            Vector3Int posOfGrid = grid.WorldToCell(worldPosition);
            Vector3 posOfWorld = grid.GetCellCenterWorld(posOfGrid);
            gridToWorldPositionEvent?.Invoke(posOfWorld);
            currentHeightUEvent?.Invoke(posOfGrid.y+1);
        }

        private void OnValidate()
        {
            amountOfWidthDepth = amountOfWidthDepth.ToVectorInt();
            if (grid == null) grid = GetComponent<Grid>();
            
#if UNITY_EDITOR
            EditorApplication.delayCall += () => { grid.cellSize = rackUnitSize; };
#endif

            if (boxGridGizmoDrawer == null) boxGridGizmoDrawer = GetComponent<BoxGridGizmoDrawer>();
            boxGridGizmoDrawer.SetAmountOfGrids(new Vector3Int(amountOfWidthDepth.x.ToInt(), rackUnits, amountOfWidthDepth.y.ToInt()));
            FixColliderToGrid();
        }

        /// 調整BoxCollider尺吋與位置，對齊Grid
        private void FixColliderToGrid()
        {
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;

            // 計算整體大小, 會以BoxCollider表面的位置計算Grid指標而會計算到43，所以要讓高度減一個高度
            Vector3 totalSize = new Vector3(grid.cellSize.x * amountOfWidthDepth.x, grid.cellSize.y * (rackUnits - 1),
                grid.cellSize.z * amountOfWidthDepth.y);
            // 設定 BoxCollider 大小
            boxCollider.size = totalSize;
            // 因為 BoxCollider 的中心在中間，所以要讓它往 Grid 的一半方向偏移
            boxCollider.center = new Vector3(totalSize.x * 0.5f, totalSize.y * 0.5f, totalSize.z * 0.5f);
        }
    }
}