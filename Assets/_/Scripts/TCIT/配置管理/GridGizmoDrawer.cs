using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace _VictorDev.GimzoUtils
{
    /// 依照Grid的CellSize，繪制Gizmo格數
    /// <para> + 目前僅支援Rectangle型態的Grid </para>
    [ExecuteAlways]
    [RequireComponent(typeof(Grid))]
    public class GridGizmoDrawer : MonoBehaviour
    {
        #region Variables

        [Label("繪製Grid數量"), MinValue(1), SerializeField]
        private Vector3Int amountOfGrids = new(3, 3, 3);
        
        public Vector3Int AmountOfGrids => amountOfGrids;

        [Foldout("[Gizmo設定]"), Label("是否始終顯示Gizmo"), SerializeField]
        private bool isAlwaysDisplayGizmo;

        [Foldout("[Gizmo設定]"), Label("是否顯示GridIndex(較耗資源)"), SerializeField]
        private bool isShowGridIndex = true;

        [Foldout("[Gizmo設定]"), SerializeField, Label("Offset顯示Grid指標")]
        private Vector3 offsetDisplayGridIndex = Vector3.zero;

        [Foldout("[Gizmo設定]"), SerializeField] private Color lineColor = Color.orange;
        [Foldout("[組件]"), SerializeField] private Grid grid;

        #endregion

        /// 設置繪製Grid數量 (X, Y, Z)軸
        public void SetAmountOfGrids(Vector3Int value) => amountOfGrids = value;

        private void OnValidate()
        {
            if (grid == null) grid = GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
        }

        #region 畫Gizmos

        private void OnDrawGizmos()
        {
            if (isAlwaysDisplayGizmo || Selection.activeGameObject == gameObject) DrawGrid();
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

            Vector3 cellSize = grid.cellSize;

            for (int x = 0; x < amountOfGrids.x; x++)
            {
                for (int y = 0; y < amountOfGrids.y; y++)
                {
                    for (int z = 0; z < amountOfGrids.z; z++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, z);
                        // 使用本地座標，不要用 GetCellCenterWorld
                        Vector3 localPos = grid.CellToLocalInterpolated(cellPos + Vector3.one * 0.5f);

                        Gizmos.DrawWireCube(localPos, cellSize);

                        if (isShowGridIndex)
                            Handles.Label(transform.TransformPoint(localPos + offsetDisplayGridIndex), $"X:{x}\tY:{y}\tZ:{z}");
                    }
                }
            }

            // 恢復矩陣
            Gizmos.matrix = oldMatrix;
        }
        #endregion
    }
}