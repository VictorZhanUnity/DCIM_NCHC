using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class WebGLFileDownloader: MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFileFromBytes(
        byte[] data,
        int length,
        string fileName,
        string mimeType
    );
#endif

    public void SaveFile(byte[] bytes) => SaveFile(bytes, "report.xlsx",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    public void SaveFile(
        byte[] bytes,
        string fileName,
        string mimeType = "application/octet-stream"
    )
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (bytes == null || bytes.Length == 0)
        {
            Debug.LogWarning("No data to download.");
            return;
        }

        DownloadFileFromBytes(bytes, bytes.Length, fileName, mimeType);
#else
        Debug.LogWarning("SaveFile is WebGL only.");
#endif
    }
}