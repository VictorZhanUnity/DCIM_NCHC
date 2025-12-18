using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.TCIT.DCIM;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using _VictorDev.DebugUtils;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM
{
    /// 資產資料父類別
    /// <para>+ [NCHC+TAINAN+IDCCO+02F+211+DCS+Synology-SA3200D 12-Bay-RackStation-2U: Synology-SA3200D 12-Bay-RackStation-2U+28]</para>
    public abstract class RevitAssetData
    {
        #region 固定欄位
        [JsonProperty] [field: SerializeField] public string deviceCode { get; protected set; }
        [JsonProperty] [field: SerializeField] public string code { get; protected set; }
        [JsonProperty] [field: SerializeField] public string deviceId { get; protected set; }
        [JsonProperty] [field: SerializeField] public string Manufacturer { get; protected set; }
        [JsonProperty] [field: SerializeField] public string modelNumber { get; protected set; }
        [JsonProperty] [field: SerializeField] public string description { get; protected set; }
        [JsonProperty] [field: SerializeField] public string system { get; protected set; }
        [JsonProperty] [field: SerializeField] public string type { get; protected set; }
        
        [field: SerializeField] public Transform Model { get; private set; }

        /// COBie資訊
        [JsonProperty] [field: SerializeField] public Information Information;
        #endregion
        
        /// 模型MeshRenderer, 以方便更改Material
        public MeshRenderer ModelMeshRender
        {
            get
            {
                if (Model == null)
                {
                    Debug.LogWarning($"Model is null: {deviceId}");
                    return null;
                }
                return render ??= Model.GetComponent<MeshRenderer>();
            }
        }
        private MeshRenderer render;

        /// 公司財產編號 (暫定)
        [field: SerializeField]
        public string CompanyAssetNo
        {
            get
            {
                if (string.IsNullOrEmpty(companyAssetNo))
                {
                    companyAssetNo = "NCHC202512" + Random.Range(0, 99999999).ToString("D8");
                }
                return companyAssetNo;
            }
        }
        private string companyAssetNo;

        /// 資產類型 Rack, Server, Router, Switch
        public EnumRevitAssetKind RevitAssetKind;

        /// 資產類型 中文
        public string DeviceKindZh;

        /// 設備名稱
        public string DeviceName;

        /// 設備名稱與流水號
        public string DeviceNameAndCode;

        public int Watt => Information.watt_limit;
        public int Weight => Information.weight_limit;
        public int HeightU => Information.heightU;
        
        /// 取得設備名稱與流水號
        public virtual void ParseDeviceNameAndCode()
        {
            RevitAssetKind = DcimHelper.GetDeviceKind(deviceId);
            DeviceKindZh = DcimHelper.GetDeviceKindZh(RevitAssetKind);
            DeviceName = DcimHelper.GetDeviceName(deviceId);
            DeviceNameAndCode = DcimHelper.GetDeviceName(deviceId, true);
        }

        /// 從Transform列表裡依照name設定模型，與設定RevitAssetDataHolder
        public void SetModelFromList(List<Transform> modelList)
        {
            Transform result = modelList.FirstOrDefault(model=>
            {
                var devicePath = DcimHelper.GetDeviceId(model.name);
                return DcimHelper.GetDeviceName(devicePath,true) == DeviceNameAndCode;
            });
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
        public void SetDevicePath(string str) => deviceId = str;
        public void SetDeviceName(string str) => DeviceName = str;
    }
}