using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _VictorDev.DebugUtils;
using _VictorDev.DoTweenUtils;
using TMPro;
using UnityEngine;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.TextUtils
{
    public static class TextHelper
    {
        public static void EventCheckIsInputHaveValue(List<TMP_InputField> inputFields, Action<bool> onCheckHandler)
            => inputFields.ForEach(target 
                => target.onValueChanged.AddListener(_=> onCheckHandler?.Invoke(IsInputHaveValue(inputFields))));

        public static bool IsInputHaveValue(List<TMP_InputField> inputFields)
            => inputFields.All(target => string.IsNullOrEmpty(target.text.Trim())) == false;

        
        /// 依T類別對像裡的變數名稱，丟給對應Comp名稱的text裡
        public static void SetParamsToTxtCompsOLD<T>(T target, List<TextDotweener> txtComps, string compHeader="Txt")
        {
            if (target == null || txtComps == null || txtComps.Count == 0)
            {
                Debug.LogWarning("SetParamsToTxtComps: target 或 txtComps 為空。");
                return;
            }

            var type = target.GetType();

            // --- 處理 Fields ---
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                string targetName = compHeader + field.Name;
                var textComp = txtComps.FirstOrDefault(comp => comp.name == targetName);
                if (textComp != null)
                {
                    object value = field.GetValue(target);
                    textComp.text = value?.ToString() ?? string.Empty;
                }
                else
                {
                    //Debug.LogWarning($"找不到對應的 Text: {targetName} (Field) / {type.Name}");
                }
            }

            // --- 處理 Properties ---
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                // 只處理有 public getter 的屬性
                if (!prop.CanRead || prop.GetGetMethod(false) == null)
                    continue;

                string targetName = compHeader + prop.Name;
                var textComp = txtComps.FirstOrDefault(comp => comp.name == targetName);
                if (textComp != null)
                {
                    object value = prop.GetValue(target);
                    textComp.text = value?.ToString() ?? string.Empty;
                }
                else
                {
                   // Debug.LogWarning($"找不到對應的 Text: {targetName} (Property) / {type.Name}");
                }
            }
        }
        
        
        public static void SetParamsToTxtComps<T, TText>(T target, List<TText> txtComps, string compHeader = "Txt")
        {
            if (target == null || txtComps == null || txtComps.Count == 0)
            {
                Debug.LogWarning("SetParamsToTxtComps: target 或 txtComps 為空。");
                return;
            }

            var type = target.GetType();
            var textType = typeof(TText);

            var nameProp = textType.GetProperty("name");
            var textProp = textType.GetProperty("text");

            if (nameProp == null || textProp == null)
            {
                Debug.LogError($"SetParamsToTxtComps: {textType.Name} 缺少 name 或 text 屬性。");
                return;
            }

            void ApplyValue(string name, object value)
            {
                var comp = txtComps.FirstOrDefault(c => (string)nameProp.GetValue(c) == name);
                if (comp != null)
                    textProp.SetValue(comp, value?.ToString() ?? string.Empty);
            }

            // --- Fields ---
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                ApplyValue(compHeader + field.Name, field.GetValue(target));

            // --- Properties ---
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || prop.GetGetMethod(false) == null)
                    continue;
                ApplyValue(compHeader + prop.Name, prop.GetValue(target));
            }
        }

    }
}