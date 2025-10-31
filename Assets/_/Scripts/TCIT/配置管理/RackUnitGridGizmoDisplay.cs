using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.TCIT.DCIM
{
    /// [Editor模式] - 繪製機櫃裡的RackUnit尺吋
    [RequireComponent(typeof(Grid))]
    public class RackUnitGridGizmoDisplay : MonoBehaviour
    {
        #region Variables

        [Foldout("[設定]"), Label("RackUnit層數"), SerializeField, Range(19, 46)] private int totalRu = 42;
        [Foldout("[設定]"), SerializeField] private Color gridGizmoColor = Color.green;
        [Foldout("[組件]"), SerializeField] private Grid rackGrid;
        [Foldout("[組件]"), SerializeField] private MeshRenderer meshRenderer;

        #endregion

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
            if (meshRenderer == null) return;

            Bounds bounds = meshRenderer.bounds;
            float unitHeight = DcimHelper.RackUnitSize.y;
            GizmoHelper.DrawRackUGizmos(bounds, unitHeight, totalRu, gridGizmoColor);
        }
#endif

        private void Awake() => meshRenderer.enabled = false;

        private void OnValidate()
        {
            if (rackGrid == null) rackGrid = GetComponent<Grid>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        }
        #endregion
    }
}