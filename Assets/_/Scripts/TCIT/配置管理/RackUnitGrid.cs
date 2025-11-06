using _VictorDev.ApiExtensions;
using _VictorDev.GimzoUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT
{
    /// [配置管理] - 機櫃空間Grid
    [RequireComponent(typeof(GridPositionCounter), typeof(GridGizmoDrawer), typeof(Grid))]
    public class RackUnitGrid : MonoBehaviour
    {
        #region Variables

        [SerializeField, Range(1, 49), Label("機櫃U層數")] private int rackUnits = 42;

        [Foldout("[Event] 換算Grid座標")] public UnityEvent<Vector3> toGridWorldPositionEvent;
        [Foldout("[Event] 目前第幾U")] public UnityEvent<int> currentHeightUEvent;

        [Foldout("[設定]"), SerializeField, Min(0.0001f), Label("單一U層尺吋")]
        private Vector3 rackUnitSize = DcimHelper.RackUnitSize;
        [Foldout("[設定]"), SerializeField, Min(1), Label("寬度/深度格數")]
        private Vector2 amountOfWidthDepth = Vector2.one;

        [Foldout("[組件]"), SerializeField] private GridPositionCounter gridPositionCounter;
        [Foldout("[組件]"), SerializeField] private GridGizmoDrawer gridGizmoDrawer;
        [Foldout("[組件]"), SerializeField] private Grid grid;

        #endregion

        /// 接收換算後Grid世界座標
        public void ReceiveGridWorldPosition(Vector3 worldPosition) => toGridWorldPositionEvent?.Invoke(worldPosition);

        /// 接收目前的GridIndex
        public void ReceiveCurrentGridIndexHandler(Vector3Int gridIndex) => currentHeightUEvent?.Invoke(gridIndex.y + 1);
       
        private void OnValidate()
        {
            amountOfWidthDepth = amountOfWidthDepth.ToVectorInt();
            if (gridPositionCounter == null) gridPositionCounter = GetComponent<GridPositionCounter>();
            if (gridGizmoDrawer == null) gridGizmoDrawer = GetComponent<GridGizmoDrawer>();
            gridGizmoDrawer.SetAmountOfGrids(new Vector3Int(amountOfWidthDepth.x.ToInt(), rackUnits, amountOfWidthDepth.y.ToInt()));
            
            if (grid == null) grid = GetComponent<Grid>();
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>{grid.cellSize = rackUnitSize;};
#endif
        }
    }
}