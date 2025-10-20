using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VictorDev.Configs;
using VictorDev.InterfaceUtils;

namespace VictorDev.ApiExtensions
{
    /// 原API類別功能擴充
    public static class ListExtension
    {
        /// [Extended] - List是否為Null或Empty
        public static bool IsNullOrEmpty<T>(this List<T> self) => self == null || self.Count == 0;

        /// [Extended] - 複製一份List
        public static List<T> MakeCopyList<T>(this List<T> self) => self.Select(x => x).ToList();

        /// [Extended] - 以separator隔開，將數組全部列出來
        public static string PrintAll<T>(this List<T> self, string separator = ",") => string.Join(separator, self);

        /// [Extended] - 篩選有實作IReceiveData<TData>的對像，傳送TData資料給這些對像
        public static void ReceiveData<T, TData>(this List<T> self, TData data)
        {
            foreach (IReceiveData<TData> target in self.OfType<IReceiveData<TData>>())
            {
                target.ReceiveData(data);
            }
        }

        /// [Extended] - 篩選出 MonoBehaviour List 中所有實作了TData類別或介面的元素
        public static List<MonoBehaviour> FilterByType<TData>(this List<MonoBehaviour> self)
        {
            List<MonoBehaviour> result = new List<MonoBehaviour>();
            for (int i = 0; i < self.Count; i++)
            {
                if (self[i] is TData) result.Add(self[i]);
                else Debug.LogWarning($"{self[i].name} 並無實作 {typeof(TData).Name}");
            }

            return result;
        }

        /// [Extended] - 替換Renderer的Texture
        public static void ReplaceTexture(this List<MeshRenderer> self, Texture texture)
        {
            if (self == null || self.Count == 0 || texture == null) return;

            // 找到第一個有效的Renderer材質作為基底
            Material baseMat = null;
            foreach (var r in self)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    baseMat = r.sharedMaterial;
                    break;
                }
            }

            if (baseMat == null)
                return;

            // 創建一個新的材質實例並設定Texture
            Material sharedInstance = new Material(baseMat) { mainTexture = texture };

            // 給所有Renderer套用這個新材質實例
            foreach (var r in self)
            {
                if (r != null)
                    r.sharedMaterial = sharedInstance;
            }
        }
        
        /// [Extended] - 替換Renderer的Texture
        public static void ReplaceTexture(this MeshRenderer[] self, Texture texture) => ReplaceTexture(self.ToList(), texture);
    }
}