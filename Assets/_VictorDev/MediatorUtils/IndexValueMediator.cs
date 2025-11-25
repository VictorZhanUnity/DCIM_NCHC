using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.MediatorUtils
{
    /// Index值處理事件仲介
    public class IndexValueMediator : MonoBehaviour
    {
        #region Variables

        [Label("[Event設定] Invoke是否被選取")] public List<KeyValueData<int, UnityEvent<bool>>> indexEventSetting;

        #endregion

        /// 設定Index值
        public void SetIndexValue(int indexValue)
        {
            indexEventSetting.ForEach(keyPair =>
            {
                keyPair.Value?.Invoke(keyPair.Key == indexValue);
            });
        }
    }
}