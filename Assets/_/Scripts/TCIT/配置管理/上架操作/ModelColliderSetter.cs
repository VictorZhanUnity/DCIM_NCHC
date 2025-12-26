using System;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.InterfaceUtils;
using _VictorDev.MediatorUtils;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace _VictorDev.ObjectUtils
{
    /// 模型Collider設置
    public class ModelColliderSetter : MonoBehaviour, IReceiveData<List<Transform>>
    {
        #region Variables

        [Label("模型資料"), SerializeField] private List<Transform> modelList;
        [Label("關鍵字與Collider類型"), SerializeField] private List<ColliderSet> colliderSets;
        
        #endregion

        /// 接收模型
        public void ReceiveData(List<Transform> models) => modelList = models;

        /// 新增Collider
        [Button]
        public void AddColliderToModels()
        {
            RemoveColliderFromModels();
            
            modelList.ForEach(model =>
            {
                var match = colliderSets.FirstOrDefault(sets =>
                    model.name.Contains(sets.keyWorld, StringComparison.CurrentCultureIgnoreCase));
                if (match != null)
                {
                    // 根據 enumColliderType 新增對應的 Collider
                    switch (match.enumColliderType)
                    {
                        case EnumColliderType.Box:
                            model.AddComponent<BoxCollider>();
                            break;

                        case EnumColliderType.Mesh:
                            model.AddComponent<MeshCollider>();
                            break;
                    } 
                }
            });
        }

        /// 移除Collider
        [Button]
        public void RemoveColliderFromModels() => ObjectHelper.RemoveColliderFromObjects(modelList);
    }

    [Serializable]
    public class ColliderSet
    {
        public string keyWorld;
        public EnumColliderType enumColliderType;
    }
    
    public enum EnumColliderType
    {
        Box, Mesh
    }
}