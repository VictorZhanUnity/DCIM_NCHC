
using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using _VictorDev.InterfaceUtils;
using _VictorDev.TCIT.DCIM;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _VictorDev.DebugUtils.ScrollRectUtils
{
    /// [框架：ScrollRect列表] ScrollList 
    public abstract class BaseScrollRectList<TData> : MonoBehaviour, IReceiveData<List<TData>>
    {
        #region Variables

        [Foldout("[Event] - SelectedItem")] public UnityEvent<TData> onSelectedItemEvent;
        [Foldout("[Event] - OnTogglesValueChanged")] public UnityEvent<bool> onTogglesValueChangedEvent;
        [Foldout("[Event] - OnTogglesValueChanged")] public UnityEvent invokeTogglesIsOnEvent, invokeTogglesIsOffEvent;
        [Foldout("[Event] - MouseOver")] public UnityEvent<TData> onPointerEnterEvent;
        [Foldout("[Event] - MouseExit")] public UnityEvent onPointerExitEvent;
        
        [Foldout("[組件]"), SerializeField] private BaseScrollRectListItem<TData> listItemPrefab;
        [Foldout("[組件]"), SerializeField] protected ScrollRect scrollRect;
        [Foldout("[組件]"), SerializeField] private ToggleGroup toggleGroup;

        /// Data列表
        public List<TData> DataList { get; protected set; } = new ();

        protected List<BaseScrollRectListItem<TData>> ListItems = new();
        
        #endregion
        
        /// 設定Data列表
        public virtual void ReceiveData(List<TData> data)
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
                ListItems.Add(item);
            });
            scrollRect.verticalNormalizedPosition = 1;
        }

        public void CancelSelection() => toggleGroup.SetAllTogglesOff(true);
        
        /// 列表排序
        public void OrderByName(bool isDescending = false) 
            => ObjectHelper.SortTargetsByObjectName<BaseScrollRectListItem<TData>>(scrollRect.content, isDescending);

        #region Event Listener

        private void OnSelectedItemEvent(TData data) => onSelectedItemEvent?.Invoke(data);
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
            ListItems.ForEach(listItems =>
            {
                listItems.OnSelectedItemEvent.RemoveAllListeners();
                listItems.OnToggleValueChangedEvent.RemoveAllListeners();
                listItems.OnPointerEnterEvent.RemoveAllListeners();
                listItems.OnPointerExitEvent.RemoveAllListeners();
                ObjectHelper.Destroy(listItems.gameObject);
            });
            ListItems.Clear();
            scrollRect.verticalNormalizedPosition = 1;
        }


        protected virtual void OnEnable()
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