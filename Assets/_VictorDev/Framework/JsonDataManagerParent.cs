using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using _VictorDev.ApiExtensions;

namespace _VictorDev.Frameworks
{
    /// JSON資料處理器
    public abstract class JsonDataManagerParent<TData> : MonoBehaviour
    {
        #region Variables

        [Label("[資料項]"), SerializeField] private TData data;
        [Label("[接收器]"), SerializeField] private List<MonoBehaviour> receivers;
        [Foldout("[Event] 是否在讀取資料")] public UnityEvent<bool> isLoadingEvent;
        private List<IReceiveData<TData>> ReceiverReceivers
            => receiverTargets ??= receivers.Cast<IReceiveData<TData>>().ToList();
        private List<IReceiveData<TData>> receiverTargets;

        protected TData Data => data;
        
        #endregion

        /// 接收JSON資料
        public void ReceiveJson(string jsonString)
        {
            data = JsonConvert.DeserializeObject<TData>(jsonString);
            BeforeInvokeData();
            isLoadingEvent?.Invoke(false);
            InvokeData();
        }

        protected virtual void BeforeInvokeData()
        {
        }
        
        /// 發送資料
        public void InvokeData() => ReceiverReceivers.ForEach(receive => receivers.ReceiveData(data));
        private void OnValidate() => receivers = ObjectHelper.CheckTypeOfList<IReceiveData<TData>>(receivers);
    }
}