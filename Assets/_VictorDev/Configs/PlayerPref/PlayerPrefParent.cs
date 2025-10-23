using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace VictorDev.Configs
{
    /// PlayerPref資料存儲處理
    public abstract class PlayerPrefParent<T> : MonoBehaviour
    {
        [Foldout("[Event]")] public UnityEvent<T> onValueLoaded;
        [Foldout("[設定]"), SerializeField] private string tagName;
        [Foldout("[設定]"), SerializeField] private T defaultValue;

        protected string TagName
        {
            get
            {
                tagName = tagName.Trim();
                return tagName;
            }
        }
        protected T DefaultValue => defaultValue;

        public abstract void Receive(T value);
    }
}