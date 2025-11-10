using _VictorDev.ApiExtensions;
using _VictorDev.GimzoUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT
{
    /// [配置管理] - 機櫃空間Grid, 當作GridGizmoDrawer的仲介Mediator
    [RequireComponent(typeof(GridGizmoDrawer))]
    public class RackUnitGrid : MonoBehaviour
    {
        #region Variables

        [SerializeField, Range(1, 49), Label("機櫃U層數")]
        private int rackUnits = 42;

        [Foldout("[Event] 換算Grid座標")] public UnityEvent<Vector3> toGridWorldPositionEvent;
        [Foldout("[Event] 目前第幾U")] public UnityEvent<int> currentHeightUEvent;

        [Foldout("[設定]"), SerializeField, ReadOnly, Label("單一U層高度")]
        private float rackUnitHeight = DcimHelper.RackUnitSize.y;

        [Foldout("[設定]"), SerializeField, Min(0.0001f), Label("單一U層寬度/深度")]
        private Vector2 rackUnitWidthDepth = new(DcimHelper.RackUnitSize.x, DcimHelper.RackUnitSize.z);

        [Foldout("[設定]"), SerializeField, Min(1), Label("寬度/深度格數")]
        private Vector2 amountOfWidthDepth = Vector2.one;

        [Foldout("[組件]"), SerializeField] private GridGizmoDrawer gridGizmoDrawer;

        #endregion

        /// 接收鼠標WorldPosition
        public Vector3 ReceiveInteractWorldPosition(Vector3 worldPosition) =>
            gridGizmoDrawer.ToGridWorldPosition(worldPosition);

        /// 接收GridGizmoDrawer換算後Grid世界座標
        public void ReceiveGridWorldPosition(Vector3 worldPosition)
        {
            toGridWorldPositionEvent?.Invoke(worldPosition);
        }

        /// 接收GridGizmoDrawer目前的GridIndex
        public void ReceiveCurrentGridIndexHandler(Vector3Int gridIndex)
        {
            int posU = gridIndex.y + 1;
            currentHeightUEvent?.Invoke(posU);
        }

        private void OnValidate()
        {
            amountOfWidthDepth = amountOfWidthDepth.ToVectorInt();
            if (gridGizmoDrawer == null) gridGizmoDrawer = GetComponent<GridGizmoDrawer>();
            gridGizmoDrawer.SetAmountOfGrids(new Vector3Int(amountOfWidthDepth.x.ToInt(), rackUnits,
                amountOfWidthDepth.y.ToInt()));

            gridGizmoDrawer.SetCellSizeOfGrid(new Vector3(rackUnitWidthDepth.x, rackUnitHeight, rackUnitWidthDepth.y));
        }

        private void Reset()
        {
            OnValidate();
        }
    }
}