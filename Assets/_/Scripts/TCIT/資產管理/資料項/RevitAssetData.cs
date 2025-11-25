using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.TCIT.DCIM;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using _VictorDev.MediatorUtils;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace _VictorDev.TCIT.DCIM
{
    /// 資產資料父類別
    public abstract class RevitAssetData
    {
        #region 固定欄位
        [JsonProperty] [field: SerializeField] public string DevicePath { get; private set; }
        [field: SerializeField] public Transform Model { get; private set; }

        /// COBie資訊
        [JsonProperty]
        [field: SerializeField]
        public Information Information { get; private set; }
        #endregion

        /// 資產類型 Rack, Server, Router, Switch
        public EnumDeviceKind DeviceKind { get; protected set; }
        
        /// 資產類型 中文
        public string DeviceKindZh { get; protected set; }
        
        /// 設備名稱
        public string DeviceName { get; protected set; }

        /// 設備名稱與流水號
        public string DeviceNameAndCode { get; protected set; }

        public string Manufacturer => Information.type_manufacturer;
        public int Watt => Information.watt;
        public int Weight => Information.weight;
        public int HeightU => Information.heightU;
        
        /// 取得設備名稱與流水號
        protected virtual void ParseDeviceNameAndCode()
        {
            DeviceKind = DcimHelper.GetDeviceKind(DevicePath);
            DeviceKindZh = DcimHelper.GetDeviceKindZh(DeviceKind);
            DeviceName = DcimHelper.GetDeviceName(DevicePath);
            DeviceNameAndCode = DcimHelper.GetDeviceName(DevicePath, true);
        }

        /// 從Transform列表裡依照name設定模型，與設定RevitAssetDataHolder
        public void SetModelFromList(List<Transform> modelList)
        {
            Transform result = modelList.FirstOrDefault(model=>model.name.Contains(DeviceNameAndCode, StringComparison.OrdinalIgnoreCase));
            if (result != null)
            {
                SetModel(result);
                if (Model.TryGetComponent(out RevitAssetDataHolder assetDataHolder))
                {
                    assetDataHolder.ReceiveAssetData(this);
                }
                else
                {
                    Model.AddComponent<RevitAssetDataHolder>().ReceiveAssetData(this);
                }
            }
            else Debug.LogError($"{DeviceNameAndCode} not found.");
        }
        public void SetModel(Transform model) => Model = model;

        public void ForDemo(Transform transform)
        {
            return;
            Model = transform;
            DevicePath = DcimHelper.GetDevicePath(Model.name);
            ParseDeviceNameAndCode();
            Information = new Information();
            transform.AddComponent<RevitAssetDataHolder>().ReceiveAssetData(this);
        }
    }
}