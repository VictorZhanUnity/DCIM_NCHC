using System;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.DebugUtils.UIEventUtils
{
    public class UIEventDispatcher : MonoBehaviour
    {
        public string eventName;

        private Button btn;
        private Toggle toggle;

        private void Awake()
        {
            eventName = UIEventManager.GetFormatEventName(eventName);
            TryGetComponent(out btn);
            TryGetComponent(out toggle);
        }

        private void OnEnable()
        {
            btn?.onClick.AddListener(SubscribeEventButton);
            toggle?.onValueChanged.AddListener(SubscribeEventToggle);
        }
        private void OnDisable()
        {
            btn?.onClick.RemoveListener(SubscribeEventButton);
            toggle?.onValueChanged.RemoveListener(SubscribeEventToggle);
        }
        private void SubscribeEventButton() => UIEventManager.SubscribeEvent(eventName);
        private void SubscribeEventToggle(bool isOn) => UIEventManager.SubscribeEvent(eventName, isOn);
        private void OnValidate() => eventName = UIEventManager.GetFormatEventName(eventName);
    }
}