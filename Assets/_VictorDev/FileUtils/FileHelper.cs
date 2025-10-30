using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using _VictorDev.Configs;
using _VictorDEV.DateTimeUtils;
using JetBrains.Annotations;
using SFB;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.FileUtils
{
    public static class FileHelper
    {
        /// 彈跳出下載路徑選擇視窗 (PC版本)
        /// <para>+ 需安裝UnityStandaloneFileBrowser </para>
        /// <para>+ https://github.com/gkngkc/UnityStandaloneFileBrowser?tab=readme-ov-file </para>
        /// <returns>檔案路徑/檔名.副檔名</returns>
        public static async Task<string> SaveFileWithPopupWindow(byte[] fileBytes, string defaultFileName = null)
        {
            var extension = new[]
            {
                new ExtensionFilter("Excel files", "xlsx")
            };
            defaultFileName ??= GetDefaultFileName();
            string filePath = string.Empty;

            // 彈出儲存檔案視窗
#if UNITY_STANDALONE_WIN || UNITY_WEBGL
            filePath = StandaloneFileBrowser.SaveFilePanel("Save Excel File", "", defaultFileName, extension);
#else
            filePath = GetDownloadFilePath();
#endif
            if (!string.IsNullOrEmpty(filePath))
            {
                await File.WriteAllBytesAsync(filePath, fileBytes);
                OpenFileOrFolder(filePath);
            }
            else DebugUtils.Debug.Log("User cancelled save dialog");

            return filePath;
        }

        /// 依平台類型回傳下載檔案路徑
        public static string GetDownloadFilePath() => Application.platform == RuntimePlatform.Android
            ? Application.persistentDataPath
            : Path.Combine(Application.dataPath, "StreamingAssets");

        /// 取得預設檔案名稱
        public static string GetDefaultFileName() => $"downloadFile_{DateTime.Now:yyyyMMddHHmmss}";

        /// 依BLOB資料生成檔案
        /// <para>return string[]: [資料夾路徑][檔案名稱(包含副檔名)]</para>
        public static string[] SaveToStreamingAssetsFolder(byte[] fileData, [CanBeNull] string fileFullName = null,
            bool isAutoOpen = true)
        {
            string folderPath = GetDownloadFilePath();
            // 若資料夾不存在，建立它
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            fileFullName ??= $"DownloadFile-{DateTime.Today.ToString(DateTimeHelper.FullDateFormat)}";
            string filePath = Path.Combine(folderPath, fileFullName);
            File.WriteAllBytes(filePath, fileData);
            DebugUtils.Debug.Log($"檔案已儲存至:{filePath}", typeof(FileHelper), EmojiEnum.Download);

            if (isAutoOpen)
            {
                try
                {
                    // 開啟 Excel 檔案
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true // 讓作業系統選擇合適的應用程式來開啟
                    });
                }
                catch (Exception e)
                {
                    DebugUtils.Debug.LogError($"開啟{fileFullName}時發生錯誤: " + e.Message);
                }
            }

            return new [] { folderPath, fileFullName };
        }

        /// 從HttpContent Header裡取得回傳的資料類型
        public static EnumResponseDataType GetResponseDataTypeFromHttpHeader(HttpContent content)
        {
            string contentType = content.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(contentType)) return EnumResponseDataType.Binary;
            if (ContentTypeToEnumMap.TryGetValue(contentType, out EnumResponseDataType dataType))
                return dataType;
            // fallback
            if (contentType.StartsWith("image/"))
                return EnumResponseDataType.Image;

            if (contentType.StartsWith("text/"))
                return EnumResponseDataType.Text;
            return EnumResponseDataType.Binary;
        }

        /// 從HttpContent Header裡取得檔名.副檔名
        public static string GetFileNameFromHttpHeader(HttpContent content)
        {
            string rawFileName = content.Headers.ContentDisposition.FileName;
            if (string.IsNullOrEmpty(rawFileName)) return string.Empty;
            string fileName = rawFileName.Trim('\"'); // 去掉前後引號（有些會包雙引號）
            return fileName.Replace("/", "").Replace(":", "").Replace(" ", "");
        }

        private static readonly Dictionary<string, EnumResponseDataType> ContentTypeToEnumMap = new()
        {
            { "application/json", EnumResponseDataType.Json },
            { "application/x-www-form-urlencoded", EnumResponseDataType.WWWForm },
            { "text/plain", EnumResponseDataType.Text },
            { "text/html", EnumResponseDataType.Text },

            // Excel
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", EnumResponseDataType.Excel },
            { "application/vnd.ms-excel", EnumResponseDataType.Excel },

            // PDF
            { "application/pdf", EnumResponseDataType.PDF },

            // 圖片
            { "image/png", EnumResponseDataType.Image },
            { "image/jpeg", EnumResponseDataType.Image },
            { "image/gif", EnumResponseDataType.Image },

            // Word
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", EnumResponseDataType.Word },

            // ZIP
            { "application/zip", EnumResponseDataType.ZIP },

            // 萬用二進位檔案
            { "application/octet-stream", EnumResponseDataType.Binary },
        };

        /// 開啟檔案 / 資料夾 (Windows)
        public static void OpenFileOrFolder(string filePath)
        {
#if UNITY_STANDALONE_WIN
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to open file: {ex.Message}");
            }
#elif UNITY_WEBGL
            Application.OpenURL(filePath);
#endif
        }

        /// 將content寫入文字檔內，寫入動作可以不管檔案是否已存在
        public static void WriteStringToTextFile(string filePath, string content)
        {
            filePath = filePath.Trim();
            content = content.Trim(' ');
            try
            {
                File.AppendAllText(filePath, content);
            }
            catch (Exception e)
            {
                // 若寫檔失敗，別再用 File.AppendAllText (避免遞迴)；改用 Unity 的 console 提示
                DebugUtils.Debug.LogWarning($"寫入失敗: {e.Message}", "FileHelper");
            }
        }
    }
}