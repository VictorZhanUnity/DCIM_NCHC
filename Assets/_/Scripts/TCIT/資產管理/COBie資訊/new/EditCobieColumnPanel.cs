using System;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TextUtils.EditableTextComponent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EditCobieColumnPanel : MonoBehaviour
{
    public UnityEvent<string, string, string> onClickConfirmButtonEvent;
    
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TMP_InputField inputValue;

    private RevitAssetData revitAssetData;
    private string Key;
    private EditableText editableText;

    private void Start()
    {
        inputValue.onSubmit.AddListener((s)=>OnClickConfirmButton());
    }

    public void EditCobieColumn(RevitAssetData data, string key, string value, EditableText target)
    {
        revitAssetData = data;
        Key = key;
        editableText = target;

        txtTitle.text = target.Title;
        inputValue.text = target.Text;
    }
    
    public void OnEditColumnInfoSuccess()
    {
        ObjectHelper.SetFieldValueByName(revitAssetData.Information, Key, inputValue.text);
    }

    private void OnEnable()
    {
        inputValue.ActivateInputField();

    }

    public void OnClickConfirmButton()
    {
        onClickConfirmButtonEvent?.Invoke(revitAssetData.deviceCode, Key, inputValue.text);
    }
}
