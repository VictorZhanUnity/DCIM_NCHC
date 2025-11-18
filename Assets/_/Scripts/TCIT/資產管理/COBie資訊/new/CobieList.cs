using System;
using System.Collections.Generic;
using _VictorDev.DebugUtils;
using _VictorDev.TextUtils;
using _VictorDev.TextUtils.EditableTextComponent;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    public class CobieList : MonoBehaviour
    {
        [Foldout("[組件]"), SerializeField] private ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private EditableText listItemPrefab;
        [Foldout("[組件]"), SerializeField] private List<EditableText> editableTexts;

        private Information _informationData;

        
        public void ReceiveData(RevitAssetData data)
        {
            _informationData = data.Information;
            UpdateUI();
        }

        [Button]
        private void UpdateUI()
        {
            ClearUI();
            List<string> fieldNames = ObjectHelper.GetFieldNames<Information>("height", "heightU", "watt", "weight");

            foreach (var fieldName in fieldNames)
            {
                EditableText listItem = ObjectHelper.Instantiate(listItemPrefab, scrollRect.content);
                string columnName = DcimSysConfig.GetCobieColumnNames(fieldName);
                string value = ObjectHelper.GetValueByFiledName(_informationData, fieldName);
                listItem.SetTitle(columnName);
                listItem.SetText(value);
                editableTexts.Add(listItem);
            }
            scrollRect.verticalNormalizedPosition = 1;
        }

        [Button]
        private void ClearUI()
        {
            ObjectHelper.DestoryObjectsOfContainer(scrollRect.content);
            editableTexts.Clear();
            scrollRect.verticalNormalizedPosition = 1;
        }

        [Button]
        private void Reset() => scrollRect = GetComponentInChildren<ScrollRect>();
    }
}