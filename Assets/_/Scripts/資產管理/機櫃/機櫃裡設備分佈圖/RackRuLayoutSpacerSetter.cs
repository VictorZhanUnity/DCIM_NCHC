using _VictorDev.DebugUtils;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _VictorDev.RevitUtils
{
    /// [Editor] - 機櫃Layout列表的RU格式設置
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class RackRuLayoutSpacerSetter : MonoBehaviour
    {
        #region Variables
        [Foldout("[設定]"), SerializeField] private Transform ruSpacerListItemPrefab;
        [Foldout("[設定]"), SerializeField] private int numOfRuSpace = 42;
        private string TxtCompLeftIndexName => "TxtLeftIndex";
        private string TxtCompRightIndexName => "TxtRightIndex";
        #endregion

        [Button]
        private void BuildRuSpaceListItems()
        {
            ClearListItems();

            #if UNITY_EDITOR
            for (int i = numOfRuSpace; i > 0; i--)
            {
                Transform item = PrefabUtility.InstantiatePrefab(ruSpacerListItemPrefab, transform).GameObject().transform;
                item.Find(TxtCompLeftIndexName).GetComponent<TextMeshProUGUI>().SetText(i.ToString());
                item.Find(TxtCompRightIndexName).GetComponent<TextMeshProUGUI>().SetText(i.ToString());
                item.name = $"RuSpaceListItem - {i}";
            }
            #endif
        }

        [Button]
        private void ClearListItems() => ObjectHelper.DestoryObjectsOfContainer(transform);

        private void Start() => OnValidate();
        private void OnValidate() => enabled = false;
    }
}