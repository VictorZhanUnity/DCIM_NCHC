using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = _VictorDev.MediatorUtils.Debug;

public class CobieInfoHandler : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private Transform inputFieldContainer;
    [SerializeField] private ScrollRect scrollRect;

    private List<string> fieldNames;

    private UploadDeviceRevitAssetData uploadDeviceRevitAssetData;
    
    public void SetUploadDeviceInfoInfo(UploadDeviceRevitAssetData data)
    {
        fieldNames ??= ObjectHelper.GetFieldNames<Information>("Watt_limit", "Weight_limit", "Length", "Width", "Height",
            "HeightU", "Watt", "Weight");
        
        uploadDeviceRevitAssetData = data;

        for (int i = 0; i < inputFields.Count; i++)
        {
            try
            {
                inputFields[i].text = ObjectHelper.GetValueByFiledName(uploadDeviceRevitAssetData.Information, fieldNames[i]);
            }
            catch (Exception e)
            {
                Debug.Log(fieldNames[i] + " : " + e.Message);
            }
        }
        scrollRect.verticalNormalizedPosition = 1;
    }

    /// 將Cobie資訊更新到UploadDevice.Information資料項裡
    public void SetCobieInfoToUploadDevice()
    {
        for (int i = 0; i < fieldNames.Count; i++)
        {
            ObjectHelper.SetFieldValueByName(uploadDeviceRevitAssetData, fieldNames[i], inputFields[i].text.Trim());    
        }
    }

    [Button]
    private void FindInputFields()
    {
        inputFields = inputFieldContainer.GetComponentsInChildren<TMP_InputField>(true).ToList();
    }
}