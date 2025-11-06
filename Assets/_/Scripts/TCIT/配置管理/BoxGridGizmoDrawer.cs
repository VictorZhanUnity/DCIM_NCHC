using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace _VictorDev.GimzoUtils
{
    /// 依照Grid的CellSize，繪制Gizmo格數
    [ExecuteAlways]
    [RequireComponent(typeof(Grid))]
    public class BoxGridGizmoDrawer : MonoBehaviour
    {
        #region Variables

        [Label("繪製Grid數量"), MinValue(1), SerializeField]
        private Vector3Int amountOfGrids = new(3, 3, 3);

        [Foldout("[Gizmo設定]"), Label("是否始終顯示Gizmo"), SerializeField]
        private bool isAlwaysDisplayGizmo;

        [Foldout("[Gizmo設定]"), Label("是否顯示GridIndex(較耗資源)"), SerializeField]
        private bool isShowGridIndex;

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
        }

        #region 畫Gizmos

        private void OnDrawGizmos()
        {
            if (isAlwaysDisplayGizmo || Selection.activeGameObject == gameObject) DrawGridLine();
        }

        /// 畫Grid格線
        private void DrawGridLine()
        {
            if (grid == null) grid = GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;

            Gizmos.color = lineColor;

            // 從Cell(0,0,0)開始畫，逐格生成邊線
            for (int x = 0; x < amountOfGrids.x; x++)
            {
                for (int y = 0; y < amountOfGrids.y; y++)
                {
                    for (int z = 0; z < amountOfGrids.z; z++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, z);
                        Vector3 worldPos = grid.GetCellCenterWorld(cellPos);
                        Vector3 cellSize = grid.cellSize;
                        // 畫出格子的框線
                        Gizmos.DrawWireCube(worldPos, cellSize);
                        if (isShowGridIndex) Handles.Label(worldPos + offsetDisplayGridIndex, $"X:{x}\tY:{y}\tZ:{z}");
                    }
                }
            }
        }

        #endregion
    }
}