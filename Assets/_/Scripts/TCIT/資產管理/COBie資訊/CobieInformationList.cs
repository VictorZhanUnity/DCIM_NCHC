using System.Collections.Generic;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.TCIT
{
    /// Cobie基本資訊列表
    public class CobieInformationList : MonoBehaviour
    {
        [Foldout("[組件]"), SerializeField] private ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private CobieInformationListItem listItemPrefab;
        
        private Information informationData;
        
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
                CobieInformationListItem listItem = Instantiate(listItemPrefab, scrollRect.content);
                string columnName = DcimSysConfig.GetCobieColumnNames(fieldName);
                string value = ObjectHelper.GetValueByFiledName(informationData, fieldName);
                listItem.SetColumnAndValue(columnName, value);
            }
            
            
            scrollRect.verticalNormalizedPosition = 1;
        }

        private void ClearUI()
        {
            ObjectHelper.DestoryObjectsOfContainer(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}