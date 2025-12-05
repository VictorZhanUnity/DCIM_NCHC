using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.DoTweenUtils;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils;
using _VictorDev.UIComps;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ToolTip_RackUnitGridInfo : MonoBehaviour
{
    #region Variables

    [Foldout("[組件]"), SerializeField, Label("TextDoTween列表")] private List<TextMeshProUGUI> txtComps;
    [Foldout("[組件]"), SerializeField] private TextDotweener txtPositionU;
    [Foldout("[組件]"), SerializeField] private Speedometer speedometer;

    #endregion

    public void ReceiveRackUnitGridInfo(RackRevitAssetData rackRevitAssetData)
    {
        TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
        speedometer.SetMaxValue(rackRevitAssetData.MaxWatt);
        speedometer.SetValue(rackRevitAssetData.UsageWatt);
        txtPositionU.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    
    public void ReceiveRackUnitGridInfo(int positionU, RackRevitAssetData rackRevitAssetData)
    {
        txtPositionU.SetText(positionU.ToString());
        TextHelper.SetParamsToTxtComps(rackRevitAssetData, txtComps);
        speedometer.SetMaxValue(rackRevitAssetData.MaxWatt);
        speedometer.SetValue(rackRevitAssetData.UsageWatt);
        txtPositionU.transform.parent.gameObject.SetActive(true);
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
    private void OnValidate()
    {
        txtComps = transform.GetComponentsInChildren<TextMeshProUGUI>(true).ToList()
            .FilterByNameForKeyChars(EnumSearchType.Include, "Txt");
        speedometer = transform.GetComponentInChildren<Speedometer>();
    }
}