using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.RevitUtils;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using _VictorDev.DebugUtils;
using Debug = UnityEngine.Debug;

namespace _VictorDev.RevitUtils
{
    /// 資產資料父類別
    public abstract class AssetDataParent
    {
        [JsonProperty] [field: SerializeField] public string DevicePath { get; private set; }
        [field: SerializeField] public Transform Model { get; private set; }

        /// COBie資訊
        [JsonProperty]
        [field: SerializeField]
        public Information Information { get; private set; }

        /// 資產類型 Rack, Server, Router, Switch
        public EnumDeviceKind DeviceKind { get; private set; }
        
        /// 設備名稱
        public string DeviceName { get; private set; }

        /// 設備名稱與流水號
        public string DeviceNameAndCode { get; private set; }

        /// 取得設備名稱與流水號
        protected void ParseDeviceNameAndCode()
        {
            DeviceKind = RevitHelper.GetDeviceKind(DevicePath);
            DeviceName = RevitHelper.GetDeviceName(DevicePath);
            DeviceNameAndCode = RevitHelper.GetDeviceName(DevicePath, true);
        }

        /// 從Transform列表裡設定模型
        public void SetModelFromList(List<Transform> modelList)
        {
            Transform result = modelList.FirstOrDefault(model=>model.name.Contains(DeviceNameAndCode, StringComparison.OrdinalIgnoreCase));
            if (result != null)
            {
                Model = result;
                if (Model.TryGetComponent(out AssetDataHolder assetDataHolder))
                {
                    assetDataHolder.ReceiveAssetData(this);
                }
                else
                {
                    Model.AddComponent<AssetDataHolder>().ReceiveAssetData(this);
                }
            }
            else Debug.LogError($"{DeviceNameAndCode} not found.");
        }
    }
}