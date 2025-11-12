using System.Collections.Generic;
using System.Linq;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    public class ToolTip_UploadDevice : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private List<TextMeshProUGUI> txtComps;
        [Foldout("[組件]"), SerializeField] private TextMeshProUGUI txtPositionU;

        #endregion

        public void ReceiveUploadDeviceInfoInfo(RevitAssetDataHolder rackDataHolder)
        {
            TextHelper.SetParamsToTxtComps(rackDataHolder.RackRevitData, txtComps);
            gameObject.SetActive(true);
        }

        public void OnNonSelectRackUnitGrid()
        {
            if (gameObject.activeSelf == false) return;
            gameObject.SetActive(false);
            txtComps.ForEach(txt => txt.SetText(""));
        }

        private void OnValidate()
        {
            txtComps ??= transform.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        }
    }
}