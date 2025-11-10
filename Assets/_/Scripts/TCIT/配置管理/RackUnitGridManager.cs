using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using _VictorDev.GimzoUtils;
using _VictorDev.TCIT;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

public class RackUnitGridManager : MonoBehaviour
{
    #region Variables

    [Label("機櫃模型列表"), SerializeField] private List<Transform> rackModelList;

    [Foldout("[設定]"), SerializeField] private string rackKeyWord;
    [Foldout("[設定]"), SerializeField] private RackUnitGrid rackUnitGridPrefab;

    public Transform target;

    #endregion

    /// 接收目前Raycast到的RackUnitGrid與其座標
    public void ReceiveMouseOverRackUnitGrid(Transform hitObject, Vector3 worldPosition)
    {
        RackUnitGrid rackUnitGrid = hitObject.GetComponentInChildren<RackUnitGrid>();
        if (rackUnitGrid != null)
        {
            Vector3 gridWorldPosition = rackUnitGrid.ReceiveInteractWorldPosition(worldPosition);
            target.transform.position = gridWorldPosition;
            target.transform.rotation = hitObject.transform.rotation;
        }
        else
        {
            Debug.LogError($"hitObject {hitObject.name} does not have a RackUnitGrid component", this);
        }
    }

    /// 接收模型
    public void ReceiveModels(List<Transform> models)
    {
        rackKeyWord = rackKeyWord.Trim();
        rackModelList = models.Where(model => model.name.Contains(rackKeyWord, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// 新增RackUnitGrid到機櫃模型下
    [Button]
    private void AddRackUnitGridToModels()
    {
        rackModelList.ForEach(rackModel =>
        {
            RackUnitGrid rackUnitGrid = rackModel.GetComponentInChildren<RackUnitGrid>();
            if (rackUnitGrid == null)
            {
                rackUnitGrid = (RackUnitGrid)PrefabUtility.InstantiatePrefab(rackUnitGridPrefab, rackModel);
                rackUnitGrid.toGridWorldPositionEvent.AddListener(OnGetGridWorldPosition);
                rackUnitGrid.GetComponent<GridGizmoDrawer>().AlignToParentBottomMesh();
            }
        });
    }
    
    /// 取得機櫃的GridWorldPosition
    private void OnGetGridWorldPosition(Vector3 gridWorldPosition)
    {
        throw new NotImplementedException();
    }

    /// 將RackUnitGrid從機櫃模型裡移除
    [Button]
    private void RemoveRackUnitGridFromModels()
    {
        rackModelList.ForEach(rackModel =>
        {
            RackUnitGrid rackUnitGrid = rackModel.GetComponentInChildren<RackUnitGrid>();
            if (rackUnitGrid != null) ObjectHelper.Destroy(rackUnitGrid.gameObject);
        });
    }
}