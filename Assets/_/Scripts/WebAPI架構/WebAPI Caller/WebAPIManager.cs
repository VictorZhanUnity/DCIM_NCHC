using System;
using System.Collections.Generic;
using System.Text;
using _VictorDev.ApiExtensions;
using _VictorDev.FileUtils;
using _VictorDev.Framework.WebAPI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TCIT.DCIM.NCHC
{
    /// WebAPI管理器 - 國網中心
    public class WebAPIManager : MonoBehaviour
    {
        #region Variables

        [Foldout("[組件] - WebAPICaller"), SerializeField]
        private WebAPICaller login, rackDataFile, allRackData, pduPower, tempHumidity,
            unPublishedDevice, installDevice, uninstallDevice, moveDevice;

        [Foldout("[設定]"), SerializeField] private string buildingCode = "TAINAN";
        [Foldout("[設定]"), SerializeField] private string excelFileNameHeader = "容量報表";

        #endregion

        #region For NaughtyAttribute
        [Button]
        public void Login() => login.CallAPI();

        [Button]
        public void GetRackExcelFile() => GetRackExcelFile(null, null);
        [Button]
        public void GetAllRackData() => GetAllRackData(null, null);
        [Button]
        public void GetTempHumidity() => GetTempHumidity(null, null);
        [Button]
        public void GetUnpublishedDevices() => GetUnpublishedDevices(null, null);
        
        #endregion
        
        /// 登入取得Token
        public void Login(string account, string pw, UnityEvent<string> onSuccess = null,
            UnityEvent<string> onError = null)
        {
            login.SetFormData(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("account", account),
                new KeyValueData<string, string>("pw", pw)
            });
            login.CallAPI(onSuccess, onError);
        }
        /// 取得容量報表
        public void GetRackExcelFile(UnityEvent<string> onSuccess, UnityEvent<string> onError) => rackDataFile.CallAPI(onSuccess, onError);

        /// 讀取機櫃容量資訊清單
        public void GetAllRackData(UnityEvent<string> onSuccess, UnityEvent<string> onError) =>
            allRackData.CallAPI(onSuccess, onError);

        /// 取得機房溫濕度
        public void GetTempHumidity(UnityEvent<string> onSuccess, UnityEvent<string> onError) =>
            tempHumidity.CallAPI(onSuccess, onError);
        
        /// 取得未上架設備
        public void GetUnpublishedDevices(UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null) =>
            unPublishedDevice.CallAPI(onSuccess, onError);

        /// 取得機櫃的PDU功耗
        /// <para>+ rackDeviceCode: 機櫃deviceCode</para>
        public void GetRackPduPower(string rackDeviceCode, UnityEvent<string> onSuccess = null,
            UnityEvent<string> onError = null)
        {
            pduPower.SetParams(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("buildingCode", buildingCode),
                new KeyValueData<string, string>("deviceCode", rackDeviceCode),
            });
            pduPower.CallAPI(onSuccess, onError);
        }
        
        /// 上架設備
        public void InstallDevice(string rackDeviceCode, string containerDeviceCode, int rackLocation,
            UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null)
        {
            installDevice.SetBodyJson(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("rackDeviceCode", rackDeviceCode),
                new KeyValueData<string, string>("containerDeviceCode", containerDeviceCode),
                new KeyValueData<string, string>("rackLocation", rackLocation.ToString())
            });
            installDevice.CallAPI(onSuccess, onError);
        }

        /// 下架設備
        public void UninstallDevice(string rackDeviceCode, string containerDeviceCode, int rackLocation,
            UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null)
        {
            installDevice.SetBodyJson(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("rackDeviceCode", rackDeviceCode),
                new KeyValueData<string, string>("containerDeviceCode", containerDeviceCode),
                new KeyValueData<string, string>("rackLocation", rackLocation.ToString())
            });
            uninstallDevice.CallAPI(onSuccess, onError);
        }

        /// 移動設備
        public void MoveDevice(string fromRackDeviceCode, string targetRackDeviceCode, string containerDeviceCode,
            int fromRackLocation, int targetRackLocation, UnityEvent<string> onSuccess = null,
            UnityEvent<string> onError = null)
        {
            moveDevice.SetBodyJson(new List<KeyValueData<string, string>>()
            {
                new KeyValueData<string, string>("fromRackDeviceCode", fromRackDeviceCode),
                new KeyValueData<string, string>("targetRackDeviceCode", targetRackDeviceCode),
                new KeyValueData<string, string>("containerDeviceCode", containerDeviceCode),
                new KeyValueData<string, string>("fromRackLocation", fromRackLocation.ToString()),
                new KeyValueData<string, string>("targetRackLocation", targetRackLocation.ToString())
            });
            moveDevice.CallAPI(onSuccess, onError);
        }
        
        /// 下載Excel容量報表
        public void SaveExcelFile(byte[] bytes)
        {
            string fileName = $"{excelFileNameHeader} - {DateTime.Now:yyyyMMddhhmmss}.xlsx";
            Debug.Log($"SaveExcelFile: {fileName}");
            WebGLFileDownloader.SaveExcelFile(bytes, fileName);
        }
    }
}