using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.DoTweenUtils;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils;
using _VictorDev.UIComps;
using NaughtyAttributes;
using UnityEngine;

public class ToolTip_RackUnitGridInfo : MonoBehaviour
{
    #region Variables

    [Foldout("[組件]"), SerializeField] private List<TextDotweener> txtComps;
    [Foldout("[組件]"), SerializeField] private TextDotweener txtPositionU;
    [Foldout("[組件]"), SerializeField] private Speedometer speedometer;

    #endregion

    public void ReceiveRackUnitGridInfo(int positionU, RackRevitAssetData rackRevitAssetData)
    {
        txtPositionU.SetText(positionU.ToString());
        TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
        speedometer.SetMaxValue(rackRevitAssetData.MaxWatt);
        speedometer.SetValue(rackRevitAssetData.UsageWatt);
        gameObject.SetActive(true);
    }

    public void OnNonSelectRackUnitGrid()
    { 
        if (gameObject.activeSelf == false) return;
        gameObject.SetActive(false);
        txtComps.ForEach(txt => txt.SetText(""));
        speedometer.SetValue(0);
    }

    [Button]
    private void Reset()
    {
        txtComps = transform.GetComponentsInChildren<TextDotweener>().ToList();
        txtComps = txtComps.FilterByNameForKeywords(true, "Txt");
        speedometer = transform.GetComponentInChildren<Speedometer>();
    }
}