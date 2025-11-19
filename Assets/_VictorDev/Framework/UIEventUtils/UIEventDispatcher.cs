using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.Framework.UIEventUtils
{
    public class UIEventDispatcher : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string eventName;

        public string EventName => eventName.Trim();
        
        private Button btn;
        private Toggle toggle;

        #endregion
        
        private void Awake()
        {
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
        private void OnValidate() => eventName = eventName.Trim();
    }
}