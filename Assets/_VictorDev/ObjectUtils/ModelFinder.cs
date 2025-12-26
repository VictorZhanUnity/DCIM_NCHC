using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.MediatorUtils;
using _VictorDev.InterfaceUtils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.ObjectUtils
{
    /// 依關鍵字尋找模型
    public class ModelFinder : MonoBehaviour
    {
        #region Variables

        [Label("名稱關鍵字"), SerializeField] private string[] keyWords;
        [Label("尋獲的模型"), SerializeField] private List<Transform> foundModels;

        [Label("[接收器]"), SerializeField] private List<MonoBehaviour> receivers;


        [Foldout("[設定]"), SerializeField] private EnumSearchType searchType = EnumSearchType.Include;
        [Foldout("[設定]"), SerializeField] private Transform targetModelsParent;

        [Foldout("[設定]"), SerializeField, Label("設定LayerMask(選填)")]
        private LayerMask layerMask;

        #endregion


        [Button]
        public void FindModelsByKeywords()
        {
            foundModels = targetModelsParent.FindChildrenByKeywords(searchType, keyWords);
            receivers.Cast<IReceiveData<List<Transform>>>().ToList()
                .ForEach(receiver => receiver.ReceiveData(foundModels));
            Debug.Log($"Found {foundModels.Count} target objects.", this);
        }

        [Button]
        public void SetLayerMask()
        {
            foundModels.ForEach(target => target.gameObject.SetLayerMask(layerMask));
            Debug.Log($"SetLayerMask is done.", this);
        }

        [Button]
        private void SetChildrenByCollider()
        {
            foundModels.ForEach(target =>
            {
                
                BoxCollider box = target.GetComponent<BoxCollider>();
                Bounds boxBounds = box.bounds;

                foreach (var mr in FindObjectsOfType<MeshRenderer>())
                {
                    if (boxBounds.Intersects(mr.bounds))
                    {
                        
                        Debug.Log($"Renderer 在範圍內：{mr.name}");
                        if (mr.name.Contains("PDU"))
                        {
                            mr.transform.SetParent(target.transform);
                        }
                    }
                }
                
                /*
                BoxCollider box = target.GetComponent<BoxCollider>();

                Collider[] hits = Physics.OverlapBox(
                    box.bounds.center,
                    box.bounds.extents,
                    box.transform.rotation
                );

                foreach (var hit in hits)
                {
                    MeshRenderer mr = hit.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        Debug.Log($"抓到 Mesh 物件：{hit.name}");
                        mr.transform.SetParent(target.transform);
                    }
                }*/
            });
        }

        private void OnValidate() => receivers = ObjectHelper.CheckTypeOfList<IReceiveData<List<Transform>>>(receivers);


#if UNITY_EDITOR
        [Button]
        public void SelectObjects() => Selection.objects = foundModels.Select(t => t.gameObject).ToArray<Object>();
#endif
    }
}