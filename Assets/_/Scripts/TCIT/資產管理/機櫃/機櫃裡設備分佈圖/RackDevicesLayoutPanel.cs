using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using _VictorDev.DoTweenUtils;
using _VictorDev.Framework.ScrollRectUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM.RevitAssetModule
{
    /// [UI] 機櫃裡的設備排列
    public class RackDevicesLayoutPanel : BaseScrollRectList<DeviceRevitAssetData>
    {
        #region Variables

        [Foldout("[Event] - SelectedItem")] public UnityEvent<Transform> onInvokeSelectedModelEvent;
        [Foldout("[組件]"), SerializeField] private TextDotweener txtTotalDevices, txtServerAmount, txtRouterAmount, txtSwitchAmount;
        [Foldout("[組件]"), SerializeField] private Transform container;

        private Dictionary<EnumRevitAssetKind, int> deviceKindCounts;
        private int EachLayoutRuHeight => -40;
        
        #endregion


        public override void ReceiveData(List<DeviceRevitAssetData> data)
        {
            DataList = data;
            
            ListItems = ListItems.ClearMissingTargets();
            ListItems.ForEach(listItems =>
            {
                listItems.OnSelectedItemEvent.RemoveAllListeners();
                listItems.OnToggleValueChangedEvent.RemoveAllListeners();
                listItems.OnPointerEnterEvent.RemoveAllListeners();
                listItems.OnPointerExitEvent.RemoveAllListeners();
                ObjectHelper.Destroy(listItems.gameObject);
            });
            ListItems.Clear();
            container.RemoveAllChildren();
            scrollRect.verticalNormalizedPosition = 1;
            isNoDataEvent?.Invoke(ListItems.Count == 0);
            
            UpdateUI(container);
            UpdateListItemsPos();
            UpdateDeviceAmount();
        }

        private void UpdateListItemsPos()
        {
            ListItems.ForEach(item =>
            {
                RectTransform itemRect = item.GetComponent<RectTransform>();
                int posY = (42 - item.Data.RackLocation) * EachLayoutRuHeight;
                itemRect.anchoredPosition = itemRect.anchoredPosition.SetY(posY);
                item.OnSelectedItemEvent.AddListener((deviceRevitAssetData)=> onInvokeSelectedModelEvent?.Invoke(deviceRevitAssetData.Model));
            });
        }

        private void UpdateDeviceAmount()
        {
            deviceKindCounts = DataList.GroupCount(device => device.RevitAssetKind);
            txtTotalDevices.SetText(deviceKindCounts.Values.Sum().ToString());
            txtServerAmount.SetText(deviceKindCounts[EnumRevitAssetKind.Server].ToString());
            txtRouterAmount.SetText(deviceKindCounts[EnumRevitAssetKind.Router].ToString());
            txtSwitchAmount.SetText(deviceKindCounts[EnumRevitAssetKind.Switch].ToString());
        }
    }
}