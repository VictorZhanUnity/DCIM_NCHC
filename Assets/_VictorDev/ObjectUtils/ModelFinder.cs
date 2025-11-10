using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.DebugUtils;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
        [Foldout("[設定]"), SerializeField] private EnumColliderType enumColliderType = EnumColliderType.Box;

        #endregion


        [Button]
        public void FindTargetObjects()
        {
            foundModels = ObjectHelper.FindObjectsByKeywords(targetModelsParent, keyWords);
            foundModels = foundModels.OrderBy(model => model.name).ToList();
            onFoundModelEvent?.Invoke(foundModels);
        }

        [Button]
        public void AddColliderToObjects()
        {
            switch (enumColliderType)
            {
                case EnumColliderType.Box: 
                    ObjectHelper.AddColliderToObjects<BoxCollider>(foundModels);
                    break;
                case EnumColliderType.Mesh: 
                    ObjectHelper.AddColliderToObjects<MeshCollider>(foundModels);
                    break;
            };
        }

        [Button]
        public void RemoveColliderFromObjects() => ObjectHelper.RemoveColliderFromObjects(foundModels);

#if UNITY_EDITOR
        [Button]
        public void SelectObjects() => Selection.objects = foundModels.Select(t => t.gameObject).ToArray();
#endif

        public enum EnumColliderType
        {
            Box, Mesh
        }
    }
}