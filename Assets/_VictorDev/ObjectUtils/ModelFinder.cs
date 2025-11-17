using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.ObjectUtils
{
    /// 依關鍵字尋找模型
    public class ModelFinder : MonoBehaviour
    {
        #region Variables

        [Label("名稱關鍵字"), SerializeField] private List<string> keyWords;
        [Label("尋獲的模型"), SerializeField] private List<Transform> foundModels;

        [Foldout("[Event] 發送尋找到的模型"), SerializeField]
        public UnityEvent<List<Transform>> onFoundModelEvent;

        [Foldout("[設定]"), SerializeField] private Transform targetModelsParent;
        [Foldout("[設定]"), SerializeField] private bool isExceptKeywords = false;

        #endregion


        [Button]
        public void FindTargetObjects()
        {
            foundModels = ObjectHelper.FindObjectsByKeywords(targetModelsParent, keyWords, isExceptKeywords);
            foundModels = foundModels.OrderBy(model => model.name).ToList();
            onFoundModelEvent?.Invoke(foundModels);
            Debug.Log($"Found {foundModels.Count} target objects.");
        }

#if UNITY_EDITOR
        [Button]
        public void SelectObjects() => Selection.objects = foundModels.Select(t => t.gameObject).ToArray();
#endif

        
    }
}