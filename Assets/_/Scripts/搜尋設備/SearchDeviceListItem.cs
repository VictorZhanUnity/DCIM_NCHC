using _VictorDev.Framework.ScrollRectUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 搜尋設備 - 列表ListItem
    public class SearchDeviceListItem : BaseScrollRectListItem<DeviceRevitAssetData>
    {
        #region Variables

        [Foldout("[組件]"), SerializeField] private RevitAssetTypeDisplay revitAssetTypeDisplay;

        #endregion

        protected override void UpdateUI()
        {
            base.UpdateUI();
            revitAssetTypeDisplay.SetDeviceKind(Data.RevitAssetKind);
        }

        protected override void Reset()
        {
            base.Reset();
            revitAssetTypeDisplay = GetComponentInChildren<RevitAssetTypeDisplay>();
        }
    }
}