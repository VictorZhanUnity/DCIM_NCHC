using _VictorDev.Framework.ScrollRectUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    public class DeviceAssetList : BaseScrollRectList<DeviceRevitAssetData>
    {
        [Foldout("[Event] - SelectedItem")] public UnityEvent<Transform> onInvokeSelectedModelEvent;

        protected override void UpdateUI(Transform container = null)
        {
            base.UpdateUI(container);
            ListItems.ForEach(item =>
            {
                item.OnSelectedItemEvent.AddListener((deviceRevitAssetData)=> onInvokeSelectedModelEvent?.Invoke(deviceRevitAssetData.Model));
            });
        }
    }
}
