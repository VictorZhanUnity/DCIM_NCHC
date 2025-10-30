using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.ApiExtensions
{
    /// 原API類別功能擴充
    public static class ByteExtension
    {
        /// [Extension] - 將 byte[] 轉成 Texture2D
        public static Texture2D ToTexture2D(this byte[] self)
        {
            if (self == null || self.Length == 0)
            {
                DebugUtils.Debug.LogError("❌ Byte array is null or empty.");
                return null;
            }

            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(self)) //LoadImage會自動調整 Texture 尺寸成原圖大小
            {
                DebugUtils.Debug.Log("✅ Texture2D loaded successfully!");
                return tex;
            }
            else
            {
                DebugUtils.Debug.LogError("❌ Failed to load Texture2D from byte array.");
                Object.Destroy(tex); // 避免浪費記憶體
                return null;
            }
        }
    }
}