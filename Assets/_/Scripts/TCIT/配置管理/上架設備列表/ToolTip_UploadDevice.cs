using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.TextUtils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備列表ToolTip
    public class ToolTip_UploadDevice : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private List<TextMeshProUGUI> txtComps;

        #endregion

        public void SetUploadDeviceInfoInfo(UploadDeviceRevitAssetData data)
        {
            TextHelper.SetParamsToTxtComps(data, txtComps);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (gameObject.activeSelf == false) return;
            gameObject.SetActive(false);
            txtComps.ForEach(txt => txt.SetText(""));
        }

        [Button]
        private void Reset()
        {
            txtComps = transform.GetComponentsInChildren<TextMeshProUGUI>().ToList()
                .FilterByNameForKeywords(EnumSearchType.Include, "Txt");
        }
    }
}