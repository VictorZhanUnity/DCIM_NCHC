using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace VictorDev.TCIT.DCIM
{
    /// [Editor模式] - 繪製機櫃裡的RackUnit尺吋
    [RequireComponent(typeof(Grid), typeof(BoxCollider))]
    public class RackUnitGridGizmoVisualizer : MonoBehaviour
    {
        #region Variables
        [Label("RackUnit層數"), SerializeField, Range(1, 46)] private int totalRu = 42;

        [Foldout("[Gizmo設定]"), SerializeField] private bool isAlwaysDisplayGizmo;
        [Foldout("[Gizmo設定]"), SerializeField] private Vector3 rackUnitSize = DcimHelper.RackUnitSize;
        [Foldout("[Gizmo設定]"), SerializeField] private Color gridGizmoColor = Color.orange;
        [Foldout("[Gizmo設定]"), SerializeField, Label("顯示文字位置Offset")] private Vector3 offsetDisplayU = new (-0.6f, 0, -0.35f);

        [Foldout("[組件]"), SerializeField] private Grid grid;
        [Foldout("[組件]"), SerializeField] private BoxCollider boxCollider;

        #endregion
        
        /// 向下對齊父類別的Collider
        private void AlignToParentBottomMesh()
        {
            transform.position = transform.parent.GetComponent<MeshRenderer>().bounds.center;
            LayerMask parentLayerMask = LayerMaskHelper.GetLayerMask(transform.parent);
            
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, parentLayerMask))
            {
                transform.position = new Vector3(
                    transform.position.x,
                    hitInfo.point.y + rackUnitSize.y * 0.5f * transform.localScale.y,
                    transform.position.z
                );
            }
        }

        private void OnValidate()
        {
            gameObject.layer = 0;
            grid ??= GetComponent<Grid>();
            boxCollider ??= GetComponent<BoxCollider>();
            
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
                if (this == null) return; // 避免物件被刪除
                AdjustGrid();
            };
#endif
            AdjustBoxCollider();

            AlignToParentBottomMesh();
        }

        #region 調整Grid與BoxCollider尺吋
        //調整Grid尺吋
        private void AdjustGrid()
        {
            float parentY = Mathf.Round(transform.parent.eulerAngles.y); // 取父物件Y角
            //XY軸對調
            grid.cellSize = Mathf.Approximately(parentY, 270f) || Mathf.Approximately(parentY, 90f)
                ? new Vector3(rackUnitSize.z, rackUnitSize.y, rackUnitSize.x) : rackUnitSize;
        }

        //調整BoxCollider尺吋
        private void AdjustBoxCollider()
        {
            // 將高度依單位放大
            boxCollider.size = new Vector3(rackUnitSize.x, rackUnitSize.y * totalRu, rackUnitSize.z);
            // 若要保持下緣固定，只讓 collider 向上延伸：
            boxCollider.center = new Vector3(
                boxCollider.center.x,
                boxCollider.size.y / 2f - (boxCollider.size.y / (totalRu)) / 2f,
                boxCollider.center.z
            );
        }
        #endregion
                
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.activeGameObject == gameObject || isAlwaysDisplayGizmo)
            {
                Color sourceGizmosColor = Gizmos.color;
                Gizmos.color = gridGizmoColor;
            
                for (int i = 0; i < totalRu; i++)
                {
                    Vector3 pos = transform.position;
                    pos.y += grid.cellSize.y * i;
                    Gizmos.DrawWireCube(pos, grid.cellSize);
                    Handles.Label(pos+ offsetDisplayU, $"{i+1}U");
                }
                Gizmos.color = sourceGizmosColor;
            }
        }
#endif
    }
}