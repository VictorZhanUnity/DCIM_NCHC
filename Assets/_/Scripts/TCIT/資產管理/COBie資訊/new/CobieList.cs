using System.Collections.Generic;
using _VictorDev.MediatorUtils;
using _VictorDev.TextUtils.EditableTextComponent;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.TCIT.DCIM
{
    public class CobieList : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event]")] public UnityEvent onUpdateUIFinishEvent;
        [Foldout("[組件]"), SerializeField] private ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private EditableText listItemPrefab;
        [Foldout("[組件]"), SerializeField] private List<EditableText> editableTexts;

        private Information informationData;
        #endregion
        
        public void ReceiveData(RevitAssetData data)
        {
            informationData = data.Information;
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
                string value = informationData != null? ObjectHelper.GetValueByFiledName(informationData, fieldName):string.Empty;
                listItem.SetTitle(columnName);
                listItem.SetText(value);
                editableTexts.Add(listItem);
            }
            scrollRect.verticalNormalizedPosition = 1;
            onUpdateUIFinishEvent?.Invoke();
        }

        [Button]
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