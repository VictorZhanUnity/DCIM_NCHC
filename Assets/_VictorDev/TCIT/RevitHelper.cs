using System;
using VictorDev.DebugUtils;

namespace VictorDev.RevitUtils
{
    /// Revit相關處理
    ///                         0       1    2    3   4   5          6
    /// <para> +DevicePath範例：NCHC+TAINAN+IDCCO+02F+211+DCR+Modem Rack-MDA19: Modem Rack-MDA19+1 </para>
    public static class RevitHelper
    {
        #region 模型DevicePath相關處理

        /// 從模型名稱 取得DevicePath
        public static string GetDevicePath(string modelName)
        {
            modelName = modelName.Trim();
            int start = modelName.IndexOf('[');
            int end = modelName.LastIndexOf(']');
            
            if (start >= 0 && end > start)
            {
                return modelName.Substring(start + 1, end - start - 1);
            }
            return modelName;
        }

        /// 從DevicePath 取得專案名稱
        public static string GetProjectName(string devicePath) => devicePath.Split('+')[0];

        /// 從DevicePath 取得專案地點
        public static string GetProjectLocation(string devicePath) => devicePath.Split('+')[1];

        /// 從DevicePath 取得樓層
        public static string GetRoomFloor(string devicePath) => devicePath.Split('+')[3];

        /// 從DevicePath 取得機房代號
        public static string GetRoomCode(string devicePath) => devicePath.Split('+')[4];

        /// 從DevicePath 取得設備類型 (DCR、DCS、DCN)
        public static EnumDeviceType GetDeviceType(string devicePath) 
            => EnumHelper.GetEnumByString<EnumDeviceType>(devicePath);

        /// 從DevicePath 取得設備名稱 (是否包含流水號)
        public static string GetDeviceName(string devicePath, bool isIncludeCode = false)
            => isIncludeCode ? devicePath.Split(":")[1] : devicePath.Split('+')[6].Split(":")[0];


        /// 從DevicePath 取得設備類型 (Rack、Server、Router、Switch)
        public static EnumDeviceKind GetDeviceKind(string devicePath) 
            => EnumHelper.GetEnumByString<EnumDeviceKind>(devicePath);
        
        #endregion
    }
    
    [Serializable]
    public enum EnumDeviceType
    {
        DCR, DCN, DCS
    }

    [Serializable]
    public enum EnumDeviceKind
    {
        Unknown, Rack, Server, Router, Switch
    }
}