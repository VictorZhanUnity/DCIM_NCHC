using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.Framework.WebAPI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

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

        #endregion

        [Button]
        public void Login() => login.CallAPI();

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
        [Button]
        public void GetRackFile(UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null) => rackDataFile.CallAPI(onSuccess, onError);

        /// 讀取機櫃容量資訊清單
        [Button]
        public void GetAllRackData(UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null) =>
            allRackData.CallAPI(onSuccess, onError);

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

        [Button]
        /// 取得機房溫濕度
        public void GetTempHumidity(UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null) =>
            tempHumidity.CallAPI(onSuccess, onError);
        
        /// 取得未上架設備
        [Button]
        public void GetUnpublishedDevices(UnityEvent<string> onSuccess = null, UnityEvent<string> onError = null) =>
            unPublishedDevice.CallAPI(onSuccess, onError);

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
    }
}