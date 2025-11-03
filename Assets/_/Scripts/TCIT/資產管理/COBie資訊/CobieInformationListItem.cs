using _VictorDev.DoTweenUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT
{
    /// Cobie資訊列表ListItem
    public class CobieInformationListItem : MonoBehaviour
    {
        [Foldout("[組件]"), SerializeField] private TextDotweener txtColumnName, txtValue;
        [Foldout("[組件]"), SerializeField] private GameObject emptyValue;

        public void SetColumnAndValue(string columnName, object value)
        {
            txtColumnName.text = columnName;
            emptyValue.SetActive(value == null);
            txtValue.text = value == null ? "" : value.ToString();
        }
    }
}