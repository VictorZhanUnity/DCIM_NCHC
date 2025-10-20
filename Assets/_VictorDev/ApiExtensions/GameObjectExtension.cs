using UnityEngine;
using Debug = VictorDev.DebugUtils.Debug;

namespace VictorDev.ApiExtensions
{
    /// 原API類別功能擴充
    public static class GameObjectExtension
    {
        /// [Extended] - 刪除GameObjec (Runtime/Editor), 包含檢查是否為null
        /// <para>+ isAllowDestroyingAssets: 是否一併刪除Unity資產 (Editor環境下)</para>
        public static void ToDestroy(this GameObject self, bool isLogResult = false)
        {
            #if UNITY_EDITOR
                string selfName = self.name;
                Object.DestroyImmediate(self, false);
                if(isLogResult) Debug.Log($"GameObject: {selfName} is destroyed.", nameof(GameObjectExtension), EmojiEnum.Success);
            #else
                Object.Destroy(self);
            #endif
        }
    }
}