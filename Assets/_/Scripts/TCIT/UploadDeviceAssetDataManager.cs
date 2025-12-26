using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備資料管理器
    public class UploadDeviceAssetDataManager : JsonDataManagerParent<List<UploadDeviceRevitAssetData>>
    {
        #region Variables

        [Foldout("[Event] 在此設定擷取資料的觸發")] public UnityEvent toGetDataEvent;
        [Foldout("[組件]"), SerializeField] private ModelCombiner modelCombiner;

        #endregion

        protected override void BeforeInvokeData() => CombineDataAndModels();

        ///設定資料與模型
        private void CombineDataAndModels()
        {
            Data.ForEach(uploadDeviceData =>
            {
                Transform targetModel = modelCombiner.DeviceModels.FirstOrDefault(t =>uploadDeviceData.type.IsMatch(t.name));
                if (targetModel == null)
                {
                    Debug.LogWarning($"Cant find target model at location: {uploadDeviceData.type}");
                    return; 
                    
                }
                uploadDeviceData.SetModel(targetModel);
            });
        }
        
       
    }
}