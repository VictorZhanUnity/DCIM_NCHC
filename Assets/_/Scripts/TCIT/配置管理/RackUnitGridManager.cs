using System.Collections.Generic;
using _VictorDev.DebugUtils;
using _VictorDev.TCIT;
using NaughtyAttributes;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

public class RackUnitGridManager : MonoBehaviour
{
    #region Variables
    [Foldout("[組件]"), SerializeField] private List<RackUnitGrid> rackUnitGrids;
    #endregion

    /// 接收目前Raycast到的RackUnitGrid與其座標
    public void ReceiveMouseOverRackUnitGrid(Transform hitObject, Vector3 worldPosition)
    {
        if (hitObject.TryGetComponent(out RackUnitGrid rackUnitGrid))
        {
            rackUnitGrid.ReceiveInteractWorldPosition(worldPosition);
        }
        else
        {
            Debug.LogError($"hitObject {hitObject.name} does not have a RackUnitGrid component", this);
        }
    }

    [Button]
    public void FindAllRackUnitGrids()
    {
        rackUnitGrids = ObjectHelper.FindAllObjectOfType<RackUnitGrid>();
    }

    private void OnValidate()
    {
        FindAllRackUnitGrids();
    }
}
