using System;
using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using _VictorDev.TextUtils.EditableTextComponent;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    public class CobieList : MonoBehaviour
    {
        #region Variables

        [Label("[資料項]"), SerializeField]  private RevitAssetData revitAssetData;
        
        [Foldout("[Event]")] public UnityEvent<RevitAssetData, string, string, EditableText> onEditCobieColumnValueEvent;
        [Foldout("[組件]"), SerializeField] private ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private EditableText listItemPrefab;
        [Foldout("[組件]"), SerializeField] private List<EditableText> editableTexts;

        private List<string> fieldNames;
        
        #endregion

        private void Start()
        {
            foreach (EditableText editableText in editableTexts)
            {
                editableText.onClickEditButton.AddListener(OnClickEditButton);
            }
        }

        private void OnClickEditButton(EditableText target)
        {
          var targetIndex =  editableTexts.FindIndex(0, editableText => editableText == target);
          onEditCobieColumnValueEvent?.Invoke(revitAssetData, fieldNames[targetIndex], target.Text, target);
        }

        public void ReceiveData(RevitAssetData data)
        {
            revitAssetData = data;
            UpdateUI();
        }

        [Button]
        public void UpdateUI(bool isScrollToTop = true)
        {
            fieldNames = ObjectHelper.GetFieldNames<Information>("Watt_limit", "Weight_limit","Length", "Width", "Height", "HeightU", "Watt", "Weight");
            
            for (int i = 0; i < editableTexts.Count; i++)
            {
                try
                {
                    editableTexts[i]
                        .SetText(ObjectHelper.GetValueByFiledName(revitAssetData.Information, fieldNames[i]));
                }
                catch (Exception e)
                {
                    Debug.Log(fieldNames[i] + " : " + e.Message);
                }
            }
           if(isScrollToTop) scrollRect.verticalNormalizedPosition = 1;
        }
        
        [Button]
        private void CreatUI()
        {
            ClearUI();
            List<string> fieldNames = ObjectHelper.GetFieldNames<Information>("height", "heightU", "watt", "weight");

            foreach (var fieldName in fieldNames)
            {
                EditableText listItem = ObjectHelper.Instantiate(listItemPrefab, scrollRect.content);
                string columnName = DcimSysConfig.GetCobieColumnNames(fieldName);
                string value = revitAssetData.Information != null? ObjectHelper.GetValueByFiledName(revitAssetData.Information, fieldName):string.Empty;
                listItem.SetTitle(columnName);
                listItem.SetText(value);
                editableTexts.Add(listItem);
            }
            scrollRect.verticalNormalizedPosition = 1;
        }

        private void ClearUI()
        {
            ObjectHelper.DestoryObjectsOfContainer(scrollRect.content);
            editableTexts.Clear();
            scrollRect.verticalNormalizedPosition = 1;
        }
        
        [Button]
        private void FindComponents() => scrollRect = GetComponentInChildren<ScrollRect>();
        
        private void Reset() => FindComponents();
    }
}