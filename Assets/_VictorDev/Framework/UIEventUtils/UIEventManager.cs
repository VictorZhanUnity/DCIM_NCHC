using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using UnityEngine.Events;

namespace _VictorDev.DebugUtils.UIEventUtils
{
    public class UIEventManager : SingletonMonoBehaviour<UIEventManager>
    {
        public List<KeyValueData<string, UnityEvent>> btnEvents;
        public List<KeyValueData<string, UnityEvent<bool>>> toggleEvents;
        
        private readonly Dictionary<string, UnityEvent> dictionaryBtnEvents = new();
        private readonly Dictionary<string, UnityEvent<bool>> dictionaryToggleEvents = new();

        protected override void Awake()
        {
            base.Awake();
            btnEvents.ForEach(keyValueData =>
            {
                string key = GetFormatEventName(keyValueData.Key);
                if (!dictionaryBtnEvents.ContainsKey(key))
                    dictionaryBtnEvents.Add(key, keyValueData.Value);
                else
                    Debug.LogError($"Duplicate button event key: {key}", this);
            });
            toggleEvents.ForEach(keyValueData =>
            {
                string key = GetFormatEventName(keyValueData.Key);
                if (!dictionaryToggleEvents.ContainsKey(key))
                    dictionaryToggleEvents.Add(key, keyValueData.Value);
                else
                    Debug.LogError($"Duplicate toggle event key: {key}", this);
            });
        }

        public static void SubscribeEvent(string eventName)
        {
            eventName = GetFormatEventName(eventName);

            if (Instance.dictionaryBtnEvents.TryGetValue(eventName, out UnityEvent evt))
                evt?.Invoke();
            else
                Debug.LogError($"{eventName} is not registered in the UI event manager", Instance);
        }
        
        public static void SubscribeEvent(string eventName, bool isOn)
        {
            eventName = GetFormatEventName(eventName);
            
            if (Instance.dictionaryToggleEvents.TryGetValue(eventName, out UnityEvent<bool> evt))
                evt?.Invoke(isOn);
            else
                Debug.LogError($"{eventName} is not registered in the UI event manager", Instance);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            btnEvents.ForEach(keyValueData => keyValueData.Key = GetFormatEventName(keyValueData.Key));
            toggleEvents.ForEach(keyValueData => keyValueData.Key = GetFormatEventName(keyValueData.Key));
        }
        
        /// 事件名稱統一格式化
        public static string GetFormatEventName(string eventName) => eventName.Trim().ToLowerInvariant();
    }
}