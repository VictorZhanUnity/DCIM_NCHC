using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DoTweenUtils;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = _VictorDev.DebugUtils.Debug;

public class ToolTip_RackUnitGridInfo : MonoBehaviour
{
    #region Variables

    [Foldout("[組件]"), SerializeField] private List<TextMeshProUGUI> txtComps;
    [Foldout("[組件]"), SerializeField] private TextMeshProUGUI txtPositionU;

    #endregion

    public void ReceiveRackUnitGridInfo(int positionU, RevitAssetDataHolder rackDataHolder, Transform uploadDevice)
    {
        txtPositionU.SetText(positionU.ToString());
        TextHelper.SetParamsToTxtComps(rackDataHolder.RackRevitData, txtComps);
        gameObject.SetActive(true);
    }

    public void OnNonSelectRackUnitGrid()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (gameObject.activeSelf==false) return;
        gameObject.SetActive(false);
        txtComps.ForEach(txt=> txt.SetText(""));
    }

    private void OnValidate()
    {
        txtComps ??= transform.GetComponentsInChildren<TextMeshProUGUI>().ToList();
    }
}
