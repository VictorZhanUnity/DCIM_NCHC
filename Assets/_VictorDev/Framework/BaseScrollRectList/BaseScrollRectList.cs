
using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using _VictorDev.InterfaceUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace _VictorDev.Framework.ScrollRectUtils
{
    /// [框架：ScrollRect列表] ScrollList 
    public abstract class BaseScrollRectList<TData> : MonoBehaviour, IReceiveData<List<TData>>
    {
        #region Variables

        [Foldout("[Event] - SelectedItem")] public UnityEvent<TData> onSelectedUploadDeviceEvent;
        [Foldout("[Event] - OnTogglesValueChanged")] public UnityEvent<bool> onTogglesValueChangedEvent;
        [Foldout("[Event] - OnTogglesValueChanged")] public UnityEvent invokeTogglesIsOnEvent, invokeTogglesIsOffEvent;
        [Foldout("[Event] - MouseOver")] public UnityEvent<TData> onPointerEnterEvent;
        [Foldout("[Event] - MouseExit")] public UnityEvent onPointerExitEvent;
        
        [Foldout("[組件]"), SerializeField] private BaseScrollRectListItem<TData> listItemPrefab;
        [Foldout("[組件]"), SerializeField] private ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private ToggleGroup toggleGroup;

        /// Data列表
        public List<TData> DataList { get; protected set; }

        #endregion
        
        /// 設定Data列表
        public void ReceiveData(List<TData> data)
        {
            DataList = data;
            ClearList();
            UpdateUI();
        }
        
        protected virtual void UpdateUI()
        {
            DataList.ForEach(data =>
            {
                BaseScrollRectListItem<TData> item = Instantiate(listItemPrefab, scrollRect.content);
                item.SetData(data);
                item.SetToggleGroup(toggleGroup);
                item.OnSelectedItemEvent.AddListener(OnSelectedItemEvent);
                item.OnToggleValueChangedEvent.AddListener(OnTogglesValueChangedEvent);
                item.OnPointerEnterEvent.AddListener(OnPointerEnterEvent);
                item.OnPointerExitEvent.AddListener(OnPointerExitEvent);
            });
            scrollRect.verticalNormalizedPosition = 1;
        }

        /// 列表排序
        public void OrderByName(bool isDescending = false) 
            => ObjectHelper.SortTargetsByObjectName<BaseScrollRectListItem<TData>>(scrollRect.content, isDescending);

        #region Event Listener

        private void OnSelectedItemEvent(TData data) => onSelectedUploadDeviceEvent?.Invoke(data);
        private void OnTogglesValueChangedEvent(bool isOn)
        {
            bool isHaveToggleOn = toggleGroup.AnyTogglesOn();
            (isHaveToggleOn? invokeTogglesIsOnEvent: invokeTogglesIsOffEvent)?.Invoke();
            onTogglesValueChangedEvent?.Invoke(isHaveToggleOn);
        }

        private void OnPointerEnterEvent(TData data) => onPointerEnterEvent?.Invoke(data);
        private void OnPointerExitEvent() => onPointerExitEvent?.Invoke();

        #endregion

        /// 清空列表 
        public void ClearList()
        {
            UploadDeviceListItem[] listItems = scrollRect.content.GetComponentsInChildren<UploadDeviceListItem>();
            Array.ForEach(listItems, child =>
            {
                child.OnSelectedItemEvent.RemoveAllListeners();
                child.OnToggleValueChangedEvent.RemoveAllListeners();
                child.OnPointerEnterEvent.RemoveAllListeners();
                child.OnPointerExitEvent.RemoveAllListeners();
                ObjectHelper.Destroy(child.gameObject);
            });
            scrollRect.verticalNormalizedPosition = 1;
        }


        private void OnEnable()
        {
            scrollRect.verticalNormalizedPosition = 1;
        }

        protected void OnValidate()
        {
            scrollRect ??= GetComponentInChildren<ScrollRect>();
            toggleGroup ??= GetComponent<ToggleGroup>();
            toggleGroup ??= GetComponentInChildren<ToggleGroup>();
        }
    }
}