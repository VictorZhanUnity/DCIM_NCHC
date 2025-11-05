using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Grid))]
public class BoxGridGizmoDrawer : MonoBehaviour
{
    #region Variables

    [Label("繪製Grid數量"), MinValue(1), SerializeField] private Vector3Int amountOfGrids = new(1, 42, 1);
    [Label("是否始終顯示Gizmo"), SerializeField] private bool isAlwaysDisplayGizmo;

    [Foldout("[Gizmo設定]"), SerializeField, Label("Offset顯示Grid指標")]
    private Vector3 offsetDisplayGridIndex = new(-0.6f, 0, -0.35f);
    [Foldout("[Gizmo設定]"), SerializeField] private Color lineColor = Color.orange;
    [Foldout("[組件]"), SerializeField] private Grid grid;

    #endregion

    /// 設置繪製Grid數量
    public void SetAmountOfGrids(Vector3Int value) => amountOfGrids = value;

    private void OnDrawGizmos()
    {
        if (grid == null) grid = GetComponent<Grid>();
        grid.cellLayout = GridLayout.CellLayout.Rectangle;
        //if (isForceDrawGizmo == false && Selection.activeGameObject != gameObject) return;

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
                    Vector3 center = worldPos + cellSize * 0.5f;
                 
                    Gizmos.DrawWireCube(worldPos, cellSize);
                    Handles.Label(worldPos + offsetDisplayGridIndex, $"{x},{y},{z}");
                }
            }
        }
    }
}