using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 設備資料管理器
    public class RevitAssetDataManager : JsonDataManagerParent<List<RackRevitAssetData>>
    {
        #region Variables

        [Foldout("[Event] 在此設定擷取資料的觸發")] public UnityEvent toGetDataEvent;
        #endregion

        [Button]
        public void ToGetData()
        {
            isLoadingEvent?.Invoke(true);
            toGetDataEvent?.Invoke();
        }
    }
}