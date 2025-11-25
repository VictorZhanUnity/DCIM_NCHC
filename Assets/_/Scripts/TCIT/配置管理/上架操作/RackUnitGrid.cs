using _VictorDev.ApiExtensions;
using _VictorDev.GimzoUtils;
using NaughtyAttributes;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    /// [配置管理] - 機櫃空間Grid, 當作GridGizmoDrawer的仲介Mediator
    [RequireComponent(typeof(GridGizmoDrawer))]
    public class RackUnitGrid : MonoBehaviour
    {
        #region Variables

        [SerializeField, Range(1, 49), Label("機櫃U層數")]
        private int rackUnits = 42;

        [Foldout("[設定]"), SerializeField, ReadOnly, Label("單一U層高度")]
        private float rackUnitHeight = DcimHelper.RackUnitSize.y;

        [Foldout("[設定]"), SerializeField, Min(0.0001f), Label("單一U層寬度/深度")]
        private Vector2 rackUnitWidthDepth;

        [Foldout("[設定]"), SerializeField, Min(1), Label("寬度/深度格數")]
        private Vector2 amountOfWidthDepth = Vector2.one;

        [Foldout("[組件]"), SerializeField] private GridGizmoDrawer gridGizmoDrawer;

        /// 換算Grid的世界座標 / 所選的U層 / 機櫃資訊
        public UnityEvent<Vector3, int, RevitAssetDataHolder> OnGetCurrentGridInfoEvent { get; } = new();

        #endregion

        /// 接收鼠標WorldPosition
        public Vector3 ReceiveInteractWorldPosition(Vector3 worldPosition) =>
            gridGizmoDrawer.ToGridWorldPosition(worldPosition);
       
       
        /// 從GridGizmoDrawer送來的Grid世界座標與Grid指標
        private void OnGetCurrentGridInfo(Vector3 gridWorldPosition, Vector3Int gridIndex)
        {
            int positionU = gridIndex.y + 1;
            if (transform.TryGetComponentInParent(out RevitAssetDataHolder rackAssetDataHolder))
                OnGetCurrentGridInfoEvent?.Invoke(gridWorldPosition, positionU, rackAssetDataHolder);
            else
                Debug.LogError($"Parent dont have RevitAssetDataHolder.", this);
        }

        #region Initialized
        private void OnEnable() => gridGizmoDrawer.OnGetCurrentGridInfoEvent.AddListener(OnGetCurrentGridInfo);
        private void OnDisable() => gridGizmoDrawer.OnGetCurrentGridInfoEvent.RemoveListener(OnGetCurrentGridInfo);

        private void OnValidate()
        {
            amountOfWidthDepth = amountOfWidthDepth.ToVectorInt();
            if (rackUnitWidthDepth == Vector2.zero)
                rackUnitWidthDepth = new (DcimHelper.RackUnitSize.x, DcimHelper.RackUnitSize.z);
            if (gridGizmoDrawer == null) gridGizmoDrawer = GetComponent<GridGizmoDrawer>();
            gridGizmoDrawer.SetAmountOfGrids(new Vector3Int(amountOfWidthDepth.x.ToInt(), rackUnits,
                amountOfWidthDepth.y.ToInt()));

            gridGizmoDrawer.SetCellSizeOfGrid(new Vector3(rackUnitWidthDepth.x, rackUnitHeight, rackUnitWidthDepth.y));
        }
        #endregion
    }
}