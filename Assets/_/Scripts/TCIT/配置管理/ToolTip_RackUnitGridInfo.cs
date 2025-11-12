using System.Collections.Generic;
using System.Linq;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils;
using _VictorDev.UIComps;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip_RackUnitGridInfo : MonoBehaviour
{
    #region Variables

    [Foldout("[組件]"), SerializeField] private List<TextMeshProUGUI> txtComps;
    [Foldout("[組件]"), SerializeField] private TextMeshProUGUI txtPositionU;
    [Foldout("[組件]"), SerializeField] private Speedometer speedometer;

    #endregion

    public void ReceiveRackUnitGridInfo(int positionU, RevitAssetDataHolder rackDataHolder, Transform uploadDevice)
    {
        txtPositionU.SetText(positionU.ToString());
        TextHelper.SetParamsToTxtComps(rackDataHolder.RackRevitData, txtComps);
        speedometer.SetMaxValue(rackDataHolder.RackRevitData.MaxWatt);
        speedometer.SetValue(rackDataHolder.RackRevitData.UsageWatt);
        gameObject.SetActive(true);
    }

    public void OnNonSelectRackUnitGrid()
    { 
        if (gameObject.activeSelf == false) return;
        gameObject.SetActive(false);
        txtComps.ForEach(txt => txt.SetText(""));
        speedometer.SetValue(0);
    }

    private void OnValidate()
    {
        txtComps ??= transform.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        speedometer ??= transform.GetComponentInChildren<Speedometer>();
    }
}