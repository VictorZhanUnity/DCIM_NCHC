using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Debug = VictorDev.DebugUtils.Debug;

namespace _VictorDev.ApiExtensions
{
    /// 原API String類別功能擴充
    public static class StringExtension
    {
        /// [Extended] - 首英文字大寫
        public static string ToCapitalizeFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// [Extended] - 轉成JSON字串格式
        public static string ToJsonFormat(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            string result = null;
            try
            {
                JToken token = JToken.Parse(str);
                // 嘗試解析為 JArray（陣列）
                if (token is JArray jsonArray)
                {
                    // 如果是 JArray，進行格式化並列印
                    result = jsonArray.ToString(Formatting.Indented);
                }
                else if (token is JObject jsonObject)
                {
                    // 如果是 JObject，進行格式化並列印
                    result = jsonObject.ToString(Formatting.Indented);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON 格式錯誤: " + e.Message);
            }

            return result;
        }

        /// [Extended] - 轉成指定的Enum
        public static T ToEnum<T>(this string value) where T : struct
            => Enum.Parse<T>(value.Trim(), true);

        /// 移除開頭的HTTP方法
        public static string RemoveHttpTypeOnHeader(this string self) => 
            self.Trim().RemoveKeyWord("http://", EnumKeyWordPosition.StartWith)
                .RemoveKeyWord("https://", EnumKeyWordPosition.StartWith);

        /// 移除關鍵字 (任一處 / 開頭 / 結尾)
        public static string RemoveKeyWord(this string self, string keyword, EnumKeyWordPosition keyWordPosition = EnumKeyWordPosition.Any)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                Debug.LogWarning("keyword is null or empty!", "StringExtension", EmojiEnum.Warning);
                return self;
            }
            switch (keyWordPosition)
            {
                default:
                case EnumKeyWordPosition.Any:
                    return self.Replace(keyword, string.Empty);
                case EnumKeyWordPosition.StartWith:
                    return self.StartsWith(keyword)? self.Substring(keyword.Length) : self;
                case EnumKeyWordPosition.EndWith:
                    return self.EndsWith(keyword)? self.Substring(0, self.Length - keyword.Length) : self;
            }
        }


        /// 將含有https / http的字串，轉成有link標籤的超鏈結
        /// <para>+ 若不包含https/http的話，直接回傳原string</para>
        public static string ToUrlLinkHtmlTag(this string self)
        {
            if (string.IsNullOrEmpty(self))
                return self;
            // 改良版 Regex：可匹配 http/https，含參數、子網域、路徑
            string pattern = @"(https?:\/\/[^\s]+)";
            return Regex.Replace(self, pattern, match =>
            {
                string url = match.Value;
                // 加上正確引號的 TMP 超連結格式
                return $"<link=\"{url}\"><color=#2986cc><u>{url}</u></color></link>";
            });
        }
        
        /// 關鍵字位置
        public enum EnumKeyWordPosition
        {
            Any, StartWith, EndWith
        }
    }
}