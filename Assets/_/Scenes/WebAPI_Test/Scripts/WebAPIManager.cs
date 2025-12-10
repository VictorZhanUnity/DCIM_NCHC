using _VictorDev.Net.WebAPI;
using NaughtyAttributes;
using UnityEngine;

public class WebAPIManager : MonoBehaviour
{
    #region Varaibles

    [Foldout("[組件] - WebApiCallerHandler"), SerializeField]
    private WebApiCallHandler login, allRackDevicesData, notOnRackDevices, deviceRelocation, rackDeviceReport;
    #endregion


    /// 登入取得Token
    public void Login() => login.CallWebAPI();
    
    /// 讀取機櫃容量資訊 (全資料)
    public void GetAllRackDevicesData() => allRackDevicesData.CallWebAPI();
    
    /// 讀取未上架的 DCS、DCN 設備清單
    public void GetNotOnRackDevices() => notOnRackDevices.CallWebAPI();

    /// 設置設備資訊 (設備上 / 下架)
    public void SetDeviceRelocation()
    {
        deviceRelocation.CallWebAPI();
    } 
    
    /// 下載容量報表
    public void DownloadRackDeviceReport()
    {
        
    } 
}
