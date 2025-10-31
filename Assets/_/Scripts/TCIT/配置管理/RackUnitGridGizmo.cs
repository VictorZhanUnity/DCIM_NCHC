using NaughtyAttributes;
using UnityEngine;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT.DCIM;
using Debug = _VictorDev.DebugUtils.Debug;

namespace VictorDev.TCIT
{
    /// [Editor模式] - 繪製機櫃裡的RackUnit尺吋
    [ExecuteAlways]
    public class RackUnitGridGizmo : MonoBehaviour
    {
        #region Variables

        public Grid RackGrid => _rackGrid ??= GetComponent<Grid>();
        private Grid _rackGrid;

        [Label("RackUnit格數"), SerializeField, Range(42, 46)]
        private int totalRu = 42;

        [Foldout("[設定]"), SerializeField, Range(1, 5)]
        private int numOfWidthGrid = 1; // X軸格數

        [Foldout("[設定]"), SerializeField, Range(1, 5)]
        private int numOfDepthGrid = 1; // Z軸格數

        public int NumOfRackUnit => totalRu;
        public int NumOfWidthGrid => numOfWidthGrid;
        public int NumOfDepthGrid => numOfDepthGrid;

        #endregion


        [Foldout("[設定]"), SerializeField] private Color gridColor = Color.green;

        [Foldout("[設定]"), SerializeField, Label("位置指標(選填)")]
        private Transform posIndicator;

        public void SetIndicator(Transform indicator) => posIndicator = indicator;

        public Vector3 GetGridCenterPosition(Vector3 worldPositon)
        {
            Vector3Int gridPos = RackGrid.WorldToCell(worldPositon);
            gridPos.Clamp(Vector3Int.zero, new Vector3Int(numOfWidthGrid - 1, totalRu - 1, numOfDepthGrid - 1));

            gridPos = gridPos.AddY(-1);
            
            Vector3 targetPos = RackGrid.GetCellCenterWorld(gridPos);
            targetPos.z = RackGrid.CellToWorld(gridPos).z;

            //Debug.Log($"gridPos: {gridPos} / worlddPos: {targetPos}");

            return targetPos;
            if (posIndicator != null)
            {
                posIndicator.transform.position = targetPos;
                Debug.Log($"RackUnit: U{gridPos.y + 1}");
            }
        }

        /// 依照Grid格數與RackUnit尺吋，來改變Grid的CellSize尺吋
        private void Update()
        {
            if (RackGrid == null) return;

            Vector3 rackUnitSize = DcimHelper.RackUnitSize;

            float x = rackUnitSize.x / numOfWidthGrid;
            float y = rackUnitSize.y;
            float z = rackUnitSize.z / numOfDepthGrid;
            RackGrid.cellSize = new Vector3(x, y, z);
        }

        /// 在機櫃下建立設備
        public Transform AddDevice(Transform deviceModelAsset, int rackLocation)
        {
            Vector3Int gridPos = new Vector3Int(0, rackLocation-1, 0);
            gridPos.Clamp(Vector3Int.zero, new Vector3Int(numOfWidthGrid - 1, totalRu - 1, numOfDepthGrid - 1));
            Vector3 targetPos = RackGrid.GetCellCenterWorld(gridPos);
            targetPos.z = RackGrid.CellToWorld(gridPos).z;
            targetPos = targetPos.AddY(DcimHelper.RackUnitSize.y * -0.5f);

            Transform deviceModel = ObjectHelper.Instantiate(deviceModelAsset, transform.parent);
            deviceModel.name = $"<{rackLocation}> {DcimHelper.GetDevicePath(deviceModel.name)}";
            deviceModel.transform.position = targetPos;
            deviceModel.rotation = Quaternion.identity;
            return deviceModel;
        }

        private void OnDrawGizmos()
        {
            if (RackGrid == null)
                return;

            Gizmos.color = gridColor;

            for (int ru = 0; ru < totalRu; ru++)
            {
                for (int x = 0; x < numOfWidthGrid; x++)
                {
                    for (int z = 0; z < numOfDepthGrid; z++)
                    {
                        DrawCellWireframe(x, ru, z);
                    }
                }
            }
        }

        void DrawCellWireframe(int x, int y, int z)
        {
            // 8個格子頂點local座標
            Vector3[] corners = new Vector3[8];

            corners[0] = RackGrid.CellToLocal(new Vector3Int(x, y, z));
            corners[1] = RackGrid.CellToLocal(new Vector3Int(x + 1, y, z));
            corners[2] = RackGrid.CellToLocal(new Vector3Int(x + 1, y, z + 1));
            corners[3] = RackGrid.CellToLocal(new Vector3Int(x, y, z + 1));

            corners[4] = RackGrid.CellToLocal(new Vector3Int(x, y + 1, z));
            corners[5] = RackGrid.CellToLocal(new Vector3Int(x + 1, y + 1, z));
            corners[6] = RackGrid.CellToLocal(new Vector3Int(x + 1, y + 1, z + 1));
            corners[7] = RackGrid.CellToLocal(new Vector3Int(x, y + 1, z + 1));

            // 轉成世界座標
            for (int i = 0; i < 8; i++)
                corners[i] = transform.TransformPoint(corners[i]);

            // 畫底面
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);

            // 畫頂面
            Gizmos.DrawLine(corners[4], corners[5]);
            Gizmos.DrawLine(corners[5], corners[6]);
            Gizmos.DrawLine(corners[6], corners[7]);
            Gizmos.DrawLine(corners[7], corners[4]);

            // 畫側邊
            Gizmos.DrawLine(corners[0], corners[4]);
            Gizmos.DrawLine(corners[1], corners[5]);
            Gizmos.DrawLine(corners[2], corners[6]);
            Gizmos.DrawLine(corners[3], corners[7]);
        }
    }
}