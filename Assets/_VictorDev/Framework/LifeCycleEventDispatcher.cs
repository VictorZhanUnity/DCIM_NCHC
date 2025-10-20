using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace VictorDev.Frameworks
{
    public class LifeCycleEventDispatcher : MonoBehaviour
    {
        #region Enable/Disabled
        [Foldout("[Event] - Enable/Disabled (isEnable)")]
        public UnityEvent<bool> onEnableDisabledEvent, onEnableDisabledReverseEvent;
        [Foldout("[Event] - Enable/Disabled")]
        public UnityEvent onEnableEvent, onDisableEvent;
        private void OnEnable()
        {
            onEnableEvent?.Invoke();
            onEnableDisabledEvent?.Invoke(true);
            onEnableDisabledReverseEvent?.Invoke(false);
        }
        private void OnDisable()
        {
            onDisableEvent?.Invoke();
            onEnableDisabledEvent?.Invoke(false);
            onEnableDisabledReverseEvent?.Invoke(true);
        }
        #endregion

        [Foldout("[Event] - Start")] public UnityEvent onStartEvent;
        private void Start() => onStartEvent?.Invoke();
    }
}