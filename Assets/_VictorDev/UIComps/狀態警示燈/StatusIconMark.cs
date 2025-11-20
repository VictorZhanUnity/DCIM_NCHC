using _VictorDev.Configs;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.UIComps
{
    public class StatusIconMark : MonoBehaviour
    {
        #region Variables

        [SerializeField] private EnumRealtimeDataStatus realtimeDataStatus = EnumRealtimeDataStatus.Good;
        [Foldout("[組件]"), SerializeField] private Image imgGood, imgWarning, imgOverload, imgMissingData;

        #endregion
        
        public void SetStatusGood() => SetStatus(EnumRealtimeDataStatus.Good);
        public void SetStatusWarning() => SetStatus(EnumRealtimeDataStatus.Warning);
        public void SetStatusOverload() => SetStatus(EnumRealtimeDataStatus.Overload);
        public void SetStatusMissingData() => SetStatus(EnumRealtimeDataStatus.MissingData);

        private void SetStatus(EnumRealtimeDataStatus status)
        {
            realtimeDataStatus = status;
            UpdateUI();
        }

        private void UpdateUI()
        {
            imgGood.gameObject.SetActive(realtimeDataStatus == EnumRealtimeDataStatus.Good);
            imgWarning.gameObject.SetActive(realtimeDataStatus == EnumRealtimeDataStatus.Warning);
            imgOverload.gameObject.SetActive(realtimeDataStatus == EnumRealtimeDataStatus.Overload);
            imgMissingData.gameObject.SetActive(realtimeDataStatus == EnumRealtimeDataStatus.MissingData);
        }

        [Button]
        private void FindComponents()
        {
            imgGood = transform.Find("iconGood").GetComponent<Image>();
            imgWarning = transform.Find("iconWarning").GetComponent<Image>();
            imgOverload = transform.Find("iconOverload").GetComponent<Image>();
            imgMissingData = transform.Find("iconMissingData").GetComponent<Image>();
        }

        private void OnValidate() => UpdateUI();

        private void Reset() => FindComponents();
    }
}